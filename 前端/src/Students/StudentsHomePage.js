import { Layout, Menu, Row, Col,Dropdown,Divider, Table,Space, Spin } from 'antd';
import { DownOutlined } from '@ant-design/icons';
import { UserOutlined, LaptopOutlined, NotificationOutlined } from '@ant-design/icons';
import React from 'react';
import axios from 'axios';
import {Navigate} from "react-router-dom";
import CourseSelect from './CourseSelect';
import GetGrades from './GetGrades';
import GetTimeTable from './GetTimeTable';
import { isExpire } from '../component/HandleTokenExpire';
import GetNotice from './GetNotice';

const { SubMenu } = Menu;
const { Header, Content, Footer, Sider } = Layout;


class StudentsHomePage extends React.Component
{
    constructor(props) {
        super(props);
        this.state = {
            redirect:false,
            name:'',
            currentContent:'',
            loading:false
        };
        this.myRef = React.createRef();//创建MENU的引用
        this.Handleclick = this.Handleclick.bind(this);
        //
        this.menu = (
            <Menu>
              <Menu.Item>
                <a target="_blank" rel="noopener noreferrer" onClick={this.redirectToChangepassword.bind(this)}>
                  修改密码
                </a>
              </Menu.Item>
              <Menu.Item>
                <a target="_blank" rel="noopener noreferrer" onClick={()=>{localStorage.removeItem("token");window.location.href="/students/log"}}>
                  退出登录
                </a>
              </Menu.Item>
            </Menu>
        );
    }
    redirectToChangepassword()
    {
        window.open('/students/changePassword');
    }
    componentDidMount()
    //查看是否处于登陆状态
    {
        var jwt = localStorage.getItem("token");
        console.log(jwt);
        if(jwt==null)
        {
            this.setState({
                redirect:true
            })
        }
        else
        {
            var that = this
            this.setState({
                loading:true
            })
            axios.get("/api/students/GetUserBasicInfo",{
                headers:{"jwt":jwt}
            }).then(function(response)
            {
                console.log(response.data);  
                if(response.data.code==401||response.data.code==403)
                {
                    localStorage.removeItem("token");
                    //请求失败就删除token
                    that.setState({
                        redirect:true
                    })
                }
                that.setState({
                    name:response.data.username,
                    year:response.data.year,
                    half:response.data.half,
                    week:response.data.week,
                    loading:false
                })   
                localStorage.setItem("year",response.data.year);
                localStorage.setItem("half",response.data.half);
            })
            .catch(function (error) {
                isExpire(error);
                console.log(error);
                
                that.setState({
                    redirect:true
                })
            });
        }
    }
    ChangePages()
    {
        this.setState({
            redirect:true
        })
    }
    Handleclick(e) {
        var num = Number(e.key);
        console.log(e);
        console.log(e.key);
        console.log(e.keyPath);
        if(num==1)
        {
            this.setState({
                currentContent:"select"
            })
        }
        else if(num==2)
        {
            this.setState({
                currentContent:"timetable"
            })
        }
        else if(num==3)
        {
            this.setState({
                currentContent:"grades"
            })
        }
        else if(num==4)
        {
            this.setState({
                currentContent:"notice"
            })
        }
        console.log(this.myRef);
    }
    render()
    {
        if(this.state.redirect)
        {
            {/* transitionName="" 和 maskTransitionName="" 是去除弹框动画属性 
                不去除的话react会抛出警告
                */}
            return <Navigate 
            transitionName=""
            maskTransitionName="" to='/students/log'></Navigate>
        }
        let semester;
        if(this.state.half==0)
        {
            semester="二"
        }
        else if(this.state.half==1)
        {
            semester="一"
        }
        let contens;
        switch(this.state.currentContent)
        {
            case "select":
                contens=<CourseSelect redirect={this.ChangePages.bind(this)}/>;
                break;
            case "timetable":
                contens=<GetTimeTable redirect={this.ChangePages.bind(this)}/>;
                break;
            case "grades":
                contens=<GetGrades redirect={this.ChangePages.bind(this)}/>;
                break;
            case "notice":
                contens=<GetNotice redirect={this.ChangePages.bind(this)}/>;
                break;
            default:
                contens = null;
        }
        //style={{margin: "10px" }}style={{ padding: '24px 0' }}
        return(
           <Spin spinning={this.state.loading}>
        <Layout>
            <header>
            <Row>
            <Col  span={4} offset={11} >
                <a>{this.state.year+this.state.half-1}-{this.state.year+this.state.half}学年,第{semester}学期,第{this.state.week}周</a>
            </Col>
            <Col span={2} offset={22}>
            <Dropdown overlay={this.menu}>
                <a className="ant-dropdown-link" onClick={e => e.preventDefault()}>
                欢迎您 , {this.state.name}<DownOutlined />
                </a>
            </Dropdown>
            </Col>
            </Row>
            </header>
            <br/><br/>
            <Layout className="site-layout-background" style={{ padding: '24px 0' }}>
                <Sider className="site-layout-background" width={200} style={{margin:"0 20px 0 0"}}>
                <Menu
                    mode="inline"
                    style={{ height: '100%' }}
                    ref={this.myRef}
                    onClick={this.Handleclick}
                >
                    <SubMenu key="sub1" icon={<UserOutlined />} title="选课管理" onTitleClick={function(){return false;}}>
                    <Menu.Item key="1" >个人选课</Menu.Item>
                    <Menu.Item key="2">课表查询</Menu.Item>
                    </SubMenu>
                    <SubMenu key="sub2" icon={<LaptopOutlined />} title="成绩管理" onTitleClick={function(){return false;}}>
                    <Menu.Item key="3">成绩查询</Menu.Item>
                    </SubMenu>
                    <SubMenu key="sub3" icon={<LaptopOutlined />} title="通知管理" onTitleClick={function(){return false;}}>
                    <Menu.Item key="4">查看通知</Menu.Item>
                    </SubMenu>
                </Menu>
                </Sider>
                <Content style={{ padding: '0 24px', minHeight: 280 }}>
                {contens}
                </Content>
            </Layout>
            <Footer style={{ textAlign: 'center' }}></Footer>
        </Layout>
        </Spin> 
            );
    }
}
export default StudentsHomePage;