
import { Button, DatePicker,Table ,Select,Space, Input,Upload,Row,Spin,Modal,Form, message, Popconfirm} from 'antd';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import { isExpire } from '../component/HandleAdminsToken';
import 'moment/locale/zh-cn';
import { UploadOutlined } from '@ant-design/icons';
import locale from 'antd/lib/date-picker/locale/zh_CN';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
const {Search} = Input;
const {Option} = Select;
class TeachersManagement extends React.Component
{
    constructor(props)
    {
        super(props);
        this.myref=React.createRef();
        this.state = ({
            page:1,
            pageSize:20,
            teachers:[],
            total:0,
            visible:false,
            loading:false
        });
        this.columns1=
        [
            {
                title: '工号',
                key: 'teacherId',
                dataIndex:'teacherId'
            },
            {
                title: '姓名',
                key: 'teacherName',
                dataIndex:'teacherName'
            },
            {
                title: '专业',
                key: 'department',
                dataIndex:'department'
            },
            {
                title: '是否离职',
                key: 'isQuit',
                render: (text,record)=>{
                    return <>{record.isQuit==1?"是":"否"}</>
                }
            },
            {
                title: '操作',
                key: 'action',
                render: (text, record,index) =>{
                    console.log(record);
                    //
                    return(     
                        <Space size="middle">
                            <Popconfirm okText="是" cancelText="否" title="确认要重置密码吗" onConfirm={this.HandleResetClick.bind(this,record)}> 
                            <Button type='primary'> 重置密码 
                            </Button>
                            </Popconfirm>
                            <Popconfirm okText="是" cancelText="否" disabled={record.isQuit==1} title="确认要设置该老师离职吗" onConfirm={this.HandleQuitClick.bind(this,record)}> 
                            <Button disabled={record.isQuit==1} type='primary' danger>
                            确认离职
                            </Button>
                            </Popconfirm>
                        </Space>
                    )
                } 
            }
        ];
    }
    HandleResetClick(record)
    {
        var that = this;
        
        axios.post('/api/admins/ResetTeacherPassword',
        {
            "teacher_id":record.teacherId,
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            message.info("成功重置密码");
            console.log(response);
            console.log(response.data);
        })
        .catch(function (error) {
            if(isExpire(error))
                window.location.href="/admins/log"
            message.error("重置密码失败");
            console.log(error);
        })
        
    }
    HandleQuitClick(record)
    {
        var that = this;
        
        axios.post('/api/admins/RegisterQuitTeacher',
        {
            "teacher_id":record.teacherId,
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            message.info("设置离职状态成功");
            console.log(response);
            that.Get_teachers(that.state.page,that.state.pageSize);
        })
        .catch(function (error) {
            if(isExpire(error))
                window.location.href="/admins/log"
            message.error("设置离职状态失败");
            console.log(error);
        })
    }
    componentDidMount()
    {
        this.Get_teachers(this.state.page,this.state.pageSize);
    }
    Get_teachers( page, pageSize)
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('/api/admins/GetTeachers',
        {
            params:{"page":page,
                    "page_size":pageSize},
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            var arr = response.data.teachers;
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
            }
            console.log(arr);
            that.setState({
                teachers:arr,
                total:response.data.total,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                teachers:[],
                total:0,
                loading:false
            })
            const arr = Object.keys(error);
            console.log(arr);
            console.log(error.response.data);
        });
    }
    handlePagination(page, pageSize)
    {
        this.setState({
            page:page,
            pageSize:pageSize
        })
        this.Get_teachers(page,pageSize);
    }
    showTotal()
    {
        //console.log(this.state.total);
        return `共 ${this.state.total} 项记录`
    }
    onSearch(value)
    {
        console.log(value=="");
        if(value == ""||value == null)
        {
            this.Get_teachers(this.state.page,this.state.pageSize);
            return;
        }
        this.setState({
            page:1
        })
        var that = this;
        axios.get('/api/admins/QueryTeacher',
        {
            params:{"teacher_id":value},
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            var arr = response.data;
            arr.key= 1;
            var new_arr = [];
            new_arr.push(arr);
            
            console.log(arr);
            that.setState({
                teachers:new_arr,
                total:1
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                teachers:[],
                total:0
            })
            const arr = Object.keys(error);
            console.log(arr);
            console.log(error.response.data);
        });
    }
    OpenaddNew()
    {
        this.setState({
            visible:true
        })
    }
    cancelModal()
    {
        this.setState({
            visible:false
        })
        //this.myref.current.resetFields();
    }
    HandleSubmit()
    {
        console.log(this.myref.current.getFieldsValue());
        const {teacherId,teacherName,department} = this.myref.current.getFieldsValue();
        var that = this;
        if(teacherId == null||teacherName == null||department == null)
        {
            return;
        }
        axios.post('/api/admins/AddTeacher',
        {
            teacher:{
                teacherId:teacherId,
                teacherName:teacherName,
                department:department,
            }     
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            message.info("成功添加老师信息");
            that.setState({
                visible:false
            })
            that.myref.current.resetFields();
            that.Get_teachers(that.state.page,that.state.pageSize);
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            message.error("添加老师信息失败");
            const arr = Object.keys(error);
            console.log(arr);
            console.log(error.response.data);
        });
    }
    HandleUploadChange(info) 
    {
        if (info.file.status == 'uploading') 
        {
            this.setState({"loading":true});
        }
        if (info.file.status !== 'uploading') 
        {
            console.log(info.file, info.fileList);
        }
        if (info.file.status === 'done') 
        {
            console.log(info);
            console.log(info.file);
            var that = this;
            message.success(` 成功添加老师信息`);
            that.Get_teachers(that.state.page,that.state.pageSize);
        } 
        else if (info.file.status === 'error') 
        {
            this.setState({"loading":false});
            message.error(`添加老师信息失败`);
        }
        
    }
    HandleUploadChange1(info) 
    {
        if (info.file.status == 'uploading') 
        {
            this.setState({"loading":true});
        }
        if (info.file.status !== 'uploading') 
        {
            console.log(info.file, info.fileList);
        }
        if (info.file.status === 'done') 
        {
            console.log(info);
            console.log(info.file);
            var that = this;
            message.success(` 成功设置离职的老师`);
            that.Get_teachers(that.state.page,that.state.pageSize);
        } 
        else if (info.file.status === 'error') 
        {
            this.setState({"loading":false});
            message.error(`设置离职的老师失败`);
        }
        
    }
    render(){
        //var that = this;
        return (
            <Spin spinning={this.state.loading}>
            <div>
                <Modal  width={800}
                okText="关闭"
                cancelText="取消"
                visible={this.state.visible} 
                onOk={this.cancelModal.bind(this)} 
                onCancel={this.cancelModal.bind(this)}
                bodyStyle={{height: '500px', overflowY: 'auto',overflowX:'auto'}}>
                <Form ref={this.myref}>

                <Form.Item  label="工号" required>
                    <Form.Item 
                    style={{ display: 'inline-flex'}}
                    name="teacherId" 
                    rules={[{ required: true, message: '学号为空' }]}
                    >
                    <Input type="number" placeholder="请入学号"/>
                    </Form.Item>
                    <Form.Item 
                    style={{ display: 'inline-flex',width: 'calc(55% - 4px)', marginLeft: '8px' }} 
                    name="teacherName" 
                    label="姓名" 
                    rules={[{ required: true, message: '姓名为空' }]}
                    >
                    <Input  placeholder="请入姓名"/>
                    </Form.Item>
                </Form.Item>

                <Form.Item 
                label="专业" 
                style={{ display: 'inline-flex', marginLeft: '8px' }} 
                name="department" 
                rules={[{ required: true, message: '专业为空' }]}
                >
                <Select 
                    style={{ width: 130 }}>
                    <Option value="软件工程">软件工程</Option>
                    <Option value="数学系">数学系</Option>
                    <Option value="物理系">物理系</Option>
                    <Option value="艺术与传媒">艺术与传媒</Option>
                </Select>
                </Form.Item>


                <br></br>
                <br></br>
                <br></br>
                
                <Form.Item
                wrapperCol={{
                    offset: 11,
                    span: 12,
                }}
                >
                <Button type="primary" htmlType="submit" onClick={this.HandleSubmit.bind(this)}>
                    确定
                </Button>
                </Form.Item>   
                </Form>
                </Modal>
                <Row>
                <Search type="number" allowClear={true} placeholder="请输入工号" onSearch={this.onSearch.bind(this)} style={{ width: 200 }} />
                &nbsp;&nbsp;&nbsp;
                <Button onClick={this.OpenaddNew.bind(this)}>添加老师</Button>
                &nbsp;&nbsp;&nbsp;
                    <Upload
                    accept=".xls, .xlsx"
                    name="file"
                    className="avatar-uploader"
                    showUploadList={false}
                    action="/api/admins/AddteachersByFile"
                    headers={{
                        'jwt':localStorage.getItem("admin_token")
                    }}
                    onChange={this.HandleUploadChange.bind(this)}
                    >
                        <Button  >
                        <UploadOutlined /> 上传文件添加老师
                        </Button>
                    </Upload>
                    &nbsp;&nbsp;&nbsp;
                    <Upload
                    accept=".xls, .xlsx"
                    name="file"
                    className="avatar-uploader"
                    showUploadList={false}
                    action="/api/admins/RegisterQuitTeachersByFile"
                    headers={{
                        'jwt':localStorage.getItem("admin_token")
                    }}
                    onChange={this.HandleUploadChange1.bind(this)}
                    >
                        <Button  >
                        <UploadOutlined /> 上传文件设置离职的老师
                        </Button>
                    </Upload>
                    &nbsp;&nbsp;&nbsp;
                    <Button>
                    <a href="http://139.224.50.124/template/添加老师信息模板.xlsx">查看模板文件1</a>
                    </Button>
                    &nbsp;&nbsp;&nbsp;
                    <Button>
                    <a href="http://139.224.50.124/template/老师离职模板.xlsx">查看模板文件2</a>
                    </Button>
                </Row>
                <br/><br/>
                <br/>
                
                <Table dataSource={this.state.teachers} 
                columns={this.columns1} 
                pagination={{
                    showSizeChanger:true,
                    onChange:this.handlePagination.bind(this),
                    defaultPageSize:20,
                    defaultCurrent:1,
                    pageSize:this.state.pageSize,
                    current:this.state.page,
                    total:this.state.total,
                    showTotal:this.showTotal.bind(this),      
                }}
                scroll={{ x: "90%" }}>
                </Table>
                
            </div>
            </Spin>
        );
    }
 
}

export default TeachersManagement;