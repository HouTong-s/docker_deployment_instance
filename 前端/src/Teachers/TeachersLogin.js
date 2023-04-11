import { Form, Input, Button, Checkbox ,Image, InputNumber, Spin, message} from 'antd';
import 'antd/dist/antd.css';
import React from 'react';
import axios from 'axios';
import {Navigate,useNavigate,useLocation} from "react-router-dom";
import { v4 as uuidv4 } from 'uuid';
class TeachersLogin extends React.Component
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
            hasLoggedin:false,
            src:'',
            loading:false
        };
    }
    componentDidMount()
    {
        if(localStorage.getItem("teacher_token")!=null)
        {
            this.setState({
                hasLoggedin:true
            })
        }
        this.Get_captcha();
    }
    Get_captcha()
    {
        const _uuid = uuidv4();
        this.setState({
            uuid:_uuid,
            loading:true
        })
        console.log(_uuid);
        var that = this;
        axios.get('/api/generatecaptcha?uuid='+_uuid)
        .then(function(response)
        {
            console.log(response.data)   
            that.setState({
                src:response.data.detail,
                loading:false
            })       
        })
        .catch(function (error) {
            that.setState({
                loading:false
            })
            console.log(error);
        });
    }
    HandleClick(e)
    {
        const {id,password,captcha} = this.myref.current.getFieldsValue();
        var that = this;
        this.setState({
            loading:true
        })
        axios.post('/api/teachers/login',
        {
            uuid:that.state.uuid,
            captcha:captcha
        },
        //第二个参数为body的内容，必须得空着
        {
            headers:{'id':id,'password':password}
        })
        .then(function (response) {
            console.log(response);
            localStorage.setItem("teacher_token",response.data.token)
            that.setState({
                hasLoggedin:true,
                loading:false
            })  
        })
        .catch(function (error) {
            that.setState({
                loading:false
            })
            that.Get_captcha();
            console.log(error.response);
            if(error.response.status==400)
            {
                message.error("验证码错误");
            }
            else
            {
                message.error("用户名或密码错误");
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
        if(this.state.hasLoggedin)
        {
            {/* transitionName="" 和 maskTransitionName="" 是去除弹框动画属性 
                不去除的话react会抛出警告
                */}
            return <Navigate 
            transitionName=""
            maskTransitionName="" to='/teachers'></Navigate>
        }
        return(
        <Spin spinning={this.state.loading}>
            <br/>
            <h1 style={{"textAlign":'center',"fontSize":"25px"}}>老师登录</h1>
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
            label="用户名"
            name="id"
            rules={[
                {
                required: true,
                message: '请输入用户名!',
                },
            ]}
            >
            <Input />
            </Form.Item>
    
            <Form.Item
            label="密码"
            name="password"
            rules={[
                {
                required: true,
                message: '请输入密码!',
                },
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
                登录
            </Button>
            </Form.Item>
        </Form>
        </Spin>
    )};
}

export default TeachersLogin;