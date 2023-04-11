import { Form, Input, Button, message ,Image, InputNumber, Spin} from 'antd';
import 'antd/dist/antd.css';
import React from 'react';
import axios from 'axios';
import {Navigate,useNavigate,useLocation} from "react-router-dom";
import { v4 as uuidv4 } from 'uuid';
import { isExpire } from '../component/HandleAdminsToken';
class AdminChangePassword extends React.Component
{
    constructor(props)
    {
        super(props);
        this.myref = React.createRef();
        this.onFinish = (values) => {
            console.log('Success:', values);
        };
        
        this.onFinishFailed = (errorInfo) => {
            console.log('Failed:', errorInfo);
        };
        this.HandleClick = this.HandleClick.bind(this);
        
        this.state={
            src:'',
            is_success:false,
            loading:false
        };
    }
    componentDidMount()
    {
        this.Get_LogStatus();
        this.Get_captcha();
    }
    Get_LogStatus()
    {
        var jwt = localStorage.getItem("admin_token");
        console.log(jwt);
        if(jwt==null)
        {
            window.location.href='/admins/log';
        }
        else
        {
            var that = this
            this.setState({
                loading:true
            })
            axios.get("/api/admins/GetUserBasicInfo",{
                headers:{"jwt":jwt}
            }).then(function(response)
            {
                console.log(response.data);  
                if(response.data.code==401||response.data.code==403)
                {
                    localStorage.removeItem("admin_token");
                    //请求失败就删除token
                    window.location.href='/admins/log';
                }
                that.setState({
                    loading:false
                })   
            })
            .catch(function (error) {
                that.setState({
                    loading:false
                })
                isExpire(error);
                    window.location.href='/admins/log';
                console.log(error);
            });
        }
    }
    Get_captcha()
    {
        const _uuid = uuidv4();
        this.setState({
            uuid:_uuid
            
        })
        console.log(_uuid);
        var that = this;
        axios.get('/api/generatecaptcha?uuid='+_uuid)
        .then(function(response)
        {
            console.log(response.data)   
            that.setState({
                src:response.data.detail
            })       
        })
        .catch(function (error) {
            console.log(error);
        });
    }
    HandleClick(e)
    {
        const {old_password,new_password,again_password,captcha} = this.myref.current.getFieldsValue();
        if(old_password==null||new_password==null||captcha==null||new_password!=again_password)
        {
            return;
        }
        var that = this;
        this.setState({
            loading:true
        })
        axios.post('/api/admins/ChangePassword',
        {
            old_password:old_password,
            new_password:new_password,
            uuid:that.state.uuid,
            captcha:captcha
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        //第三个参数才能填header
        .then(function (response) {
            console.log(response);
            that.setState({
                is_success:true,
                loading:false
            })         
        })
        .catch(function (error) {
            that.setState({
                loading:false
            })
            if(isExpire(error))
            {
                window.location.href='/admins/log';
                return;
            }
            if(error.response.status==400)
            {
                that.Get_captcha();
                message.error("验证码错误");
            }
            else
            {
                message.error("原密码错误");
            }
            const arr = Object.keys(error);
            console.log(arr);
            console.log(error.response.data);
            console.log(error.response.status);
            //that.forceUpdate();
        });
        e.preventDefault();
        //window.location.reload()这句话用来刷新页面
        
    }
    render() {
        var that = this;
        if(this.state.is_success)
        {
            {/* transitionName="" 和 maskTransitionName="" 是去除弹框动画属性 
                不去除的话react会抛出警告
                */}
            return <Navigate 
            transitionName=""
            maskTransitionName="" to='/admins'></Navigate>
        }
        return(
        <Spin spinning={this.state.loading}>
            <br/>
            <Form
            ref={this.myref}
            name="basic"
            colon
            labelCol={{
            span: 10,
            }}
            wrapperCol={{
            span: 4,
            }}
            initialValues={{
            remember: true,
            }}
            onFinish={this.onFinish}
            onFinishFailed={this.onFinishFailed}
            autoComplete="off"
                
            >
            <Form.Item
            label="原密码"
            name="old_password"
            rules={[
                {
                required: true,
                message: '请输入原密码!',
                },
            ]}
            >
            <Input.Password />
            </Form.Item>

            <Form.Item
            label="新密码"
            name="new_password"
            rules={[
                {
                required: true,
                message: '请输入新密码!',
                },
            ]}
            >
            <Input.Password />
            </Form.Item>

            <Form.Item
            label="确认密码"
            name="again_password"
            rules={[
                {
                required: true,
                message: '请再次输入密码!',
                },
                {
                    validator: (_, value) =>{
                        const {new_password,again_password} = that.myref.current.getFieldsValue();
                        if(new_password == again_password) 
                        {
                            return Promise.resolve()
                        }
                        else
                        {
                            return Promise.reject('两次输入密码不相等')
                        }
                    }
                }
                
            ]}
            >
            <Input.Password />
            </Form.Item>
            
            <Form.Item
            label="验证码"
            name="captcha"
            rules={[
                {
                required: true,
                message: '请输入验证码!',
                },
            ]}
            >
            <Input />
            </Form.Item>
            <Form.Item
            colon={false}
            label=" "
            
            >
            <Image onClick={this.Get_captcha.bind(this)} preview={false} style={{"height":"50px"}} src={this.state.src}></Image>
            </Form.Item>
            <Form.Item
            wrapperCol={{
                offset: 13,
                span: 15,
            }}
            >
            <Button type="primary" htmlType="submit" onClick={this.HandleClick}>
                确认
            </Button>
            </Form.Item>
        </Form>
    </Spin>
    )};
}

export default AdminChangePassword;