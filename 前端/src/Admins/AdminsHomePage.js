import { Layout, Menu, Row, Col,Dropdown,Input,Form,Button, Spin } from 'antd';
import { DownOutlined } from '@ant-design/icons';
import { UserOutlined, LaptopOutlined, NotificationOutlined } from '@ant-design/icons';
import React from 'react';
import axios from 'axios';
import {Navigate} from "react-router-dom";
import { isExpire } from '../component/HandleAdminsToken';
import LessonsManagement from './LessonsManagement';
import GetSchedules from './GetSchedules';
import AddSchedules from './AddSchedules';
import StudentsManagement from './StudentsManagement';
import TeachersManagement from './TeachersManagement';
import SelectSchedulesForStudent from './SelectSchedulesForStudent';
import SetSelectTime from './SetSelectTime';
import NoticesManagement from './NoticesManagement';
import SetImportGradeTime from './SetImportGradeTime';
import SetSemesterInfo from './SetSemesterInfo';

const { SubMenu } = Menu;
const { Header, Content, Footer, Sider } = Layout;


class AdminsHomePage extends React.Component
{
    constructor(props) {
        super(props);
        this.state = {
            redirect:false,
            name:'',
            currentContent:'',
            loading:false
        };
        this.myref = React.createRef();//创建MENU的引用
        this.Handleclick = this.Handleclick.bind(this);
        this.menu = (
            <Menu>
              <Menu.Item>
                <a target="_blank" rel="noopener noreferrer" onClick={this.redirectToChangepassword.bind(this)}>
                  修改密码
                </a>
              </Menu.Item>
              <Menu.Item>
                <a target="_blank" rel="noopener noreferrer" onClick={()=>{localStorage.removeItem("admin_token");window.location.href="/admins/log"}}>
                  退出登录
                </a>
              </Menu.Item>
            </Menu>
        );
    }
    redirectToChangepassword()
    {
        window.open('/admins/changePassword');
    }
    componentDidMount()
    //查看是否处于登陆状态
    {
        this.GetBasicInfo();
    }
    GetBasicInfo()
    {
        var jwt = localStorage.getItem("admin_token");
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
            axios.get("/api/admins/GetUserBasicInfo",{
                headers:{"jwt":jwt}
            }).then(function(response)
            {
                console.log(response.data);  
                if(response.data.code==401||response.data.code==403)
                {
                    localStorage.removeItem("admin_token");
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
        switch(num)
        {
            case 1:
                this.setState({
                    currentContent:"lesson"
                });
                break;
            case 2:
                this.setState({
                    currentContent:"Schedules"
                });
                break;
            case 3:
                this.setState({
                    currentContent:"AddSchedules"
                });
                break;
            case 4:
                this.setState({
                    currentContent:"StudentsManagement"
                });
                break;
            case 5:
                this.setState({
                    currentContent:"TeachersManagement"
                });
                break;
            case 6:
                this.setState({
                    currentContent:"SetSelectTime"
                });
                break;
            case 7:
                this.setState({
                    currentContent:"SelectSchedulesForStudent"
                });
                break;
            case 8:
                this.setState({
                    currentContent:"SetImportGradeTime"
                });
                break;
            case 9:
                this.setState({
                    currentContent:"Notice"
                });
                break;
            case 10:
                this.setState({
                    currentContent:"SetSemesterInfo"
                });
                break;
            default:
                break;
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
            maskTransitionName="" to='/admins/log'></Navigate>
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
        let contents;
        switch(this.state.currentContent)
        {
            case "lesson":
                contents = <LessonsManagement redirect={this.ChangePages.bind(this)}/>;
                break;
            case "Schedules":
                contents = <GetSchedules redirect={this.ChangePages.bind(this)}/>;
                break;
            case "AddSchedules":
                contents = <AddSchedules redirect={this.ChangePages.bind(this)}/>;
                break;
            case "StudentsManagement":
                contents = <StudentsManagement redirect={this.ChangePages.bind(this)} />;
                break;
            case "TeachersManagement":
                contents = <TeachersManagement redirect={this.ChangePages.bind(this)} />;
                break;
            case "SetSelectTime":
                contents = <SetSelectTime redirect={this.ChangePages.bind(this)} />;
                break;
            case "SelectSchedulesForStudent":
                contents = <SelectSchedulesForStudent redirect={this.ChangePages.bind(this)} />;
                break;
            case "Notice":
                contents=<NoticesManagement redirect={this.ChangePages.bind(this)} />;
                break;
            case "SetImportGradeTime":
                contents=<SetImportGradeTime redirect={this.ChangePages.bind(this)} />;
                break;
            case "SetSemesterInfo":
                contents=<SetSemesterInfo getbasic={this.GetBasicInfo.bind(this)} redirect={this.ChangePages.bind(this)} />;
                break;
            default:
                contents = null;
        }
        //style={{margin: "10px" }}
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
                    onClick={this.Handleclick}
                >
                    <SubMenu key="sub1" icon={<UserOutlined />} title="课程信息管理" onTitleClick={function(){return false;}}>
                        <Menu.Item key="1" >课程信息总览</Menu.Item>
                    </SubMenu>
                    <SubMenu key="sub2" icon={<LaptopOutlined />} title="排课管理" onTitleClick={function(){return false;}}>
                        <Menu.Item key="2">排课信息总览</Menu.Item>
                        <Menu.Item key="3">增加排课</Menu.Item>
                        <Menu.Item key="8">设置成绩导入时间</Menu.Item>
                    </SubMenu>
                    <SubMenu key="sub3" icon={<LaptopOutlined />} title="用户管理" onTitleClick={function(){return false;}}>
                    <Menu.Item key="4">学生信息总览</Menu.Item>
                    <Menu.Item key="5">老师信息总览</Menu.Item>
                    </SubMenu>
                    <SubMenu key="sub4" icon={<LaptopOutlined />} title="选课管理" onTitleClick={function(){return false;}}>
                    <Menu.Item key="6">设置选课时间</Menu.Item>
                    <Menu.Item key="7">代学生选课</Menu.Item>
                    </SubMenu>
                    <SubMenu key="sub5" icon={<LaptopOutlined />} title="通知管理" onTitleClick={function(){return false;}}>
                    <Menu.Item key="9">通知总览</Menu.Item>
                    </SubMenu>
                    <SubMenu key="sub6" icon={<LaptopOutlined />} title="学期信息管理" onTitleClick={function(){return false;}}>
                    <Menu.Item key="10">修改学期信息</Menu.Item>
                    </SubMenu>
                </Menu>
                </Sider>
                <Content style={{ padding: '0 20px', minHeight: 280 }}>
                {contents}
                </Content>
            </Layout>
            <Footer style={{ textAlign: 'center' }}></Footer>
            
        </Layout>
        </Spin>  
            );
    }
}
export default AdminsHomePage;