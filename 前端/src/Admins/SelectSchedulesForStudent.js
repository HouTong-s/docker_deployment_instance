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
class SelectSchedulesForStudent extends React.Component
{
    constructor(props)
    {
        super(props);
        this.myref=React.createRef();
        this.state = ({
            page:1,
            pageSize:20,
            students:[],
            total:0,
            visible:false,
            loading:false
        });
        this.columns1=
        [
            {
                title: '学号',
                key: 'studentId',
                dataIndex:'studentId'
            },
            {
                title: '姓名',
                key: 'studentName',
                dataIndex:'studentName'
            },
            {
                title: '专业',
                key: 'department',
                dataIndex:'department'
            },
            {
                title: '年级',
                key: 'inYear',
                dataIndex:'inYear'
            },
            {
                title: '身份',
                key: 'identity',
                dataIndex:'identity'
            },
            {
                title: '是否毕业',
                key: 'isGraduate',
                render: (text,record)=>{
                    return <>{record.isGraduate==1?"是":"否"}</>
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
                            <Button type='primary' onClick={this.SelectSchedule.bind(this,record)}>
                            代替新修选课
                            </Button>
                            <Button type='primary' onClick={this.SelectSchedule1.bind(this,record)}>
                            代替重修选课
                            </Button>
                        </Space>
                    )
                } 
            }
        ];
    }
    SelectSchedule(record)
    {
        window.open('/admins/selectSchedule?id='+record.studentId+'&status=1'/*,"_blank"*/);
    }
    SelectSchedule1(record)
    {
        window.open('/admins/selectSchedule?id='+record.studentId+'&status=2'/*,"_blank"*/);
    }
    componentDidMount()
    {
        this.Get_students(this.state.page,this.state.pageSize);
    }
    Get_students( page, pageSize)
    {
        var that = this;
        this.setState({
            loading:false
        })
        axios.get('/api/admins/GetStudents',
        {
            params:{"page":page,
                    "page_size":pageSize},
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            var arr = response.data.students;
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
            }
            console.log(arr);
            that.setState({
                students:arr,
                total:response.data.total,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                students:[],
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
        this.Get_students(page,pageSize);
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
            this.Get_students(this.state.page,this.state.pageSize);
            return;
        }
        this.setState({
            page:1
        })
        var that = this;
        axios.get('/api/admins/QueryStudent',
        {
            params:{"student_id":value},
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
                students:new_arr,
                total:1
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                students:[],
                total:0
            })
            const arr = Object.keys(error);
            console.log(arr);
            console.log(error.response.data);
        });
    }
    render(){
        //var that = this;
        return (
            <Spin spinning={this.state.loading}>
            <div>
                <Row>
                <Search type="number" allowClear={true} placeholder="请输入学号" onSearch={this.onSearch.bind(this)} style={{ width: 200 }} />
                &nbsp;&nbsp;&nbsp;
                </Row>
                <br/><br/>
                <br/>
                <Table dataSource={this.state.students} 
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

export default SelectSchedulesForStudent;