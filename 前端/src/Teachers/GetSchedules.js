
import { Button, DatePicker,Table ,Select,Space, Spin} from 'antd';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import { isExpire } from '../component/HandleTeachersToken';
//import { Link } from 'react-router-dom';
import 'moment/locale/zh-cn';
import locale from 'antd/lib/date-picker/locale/zh_CN';
const {Option} = Select;
class GetSchedules extends React.Component
{
    constructor(props)
    {
        super(props);
        this.state = ({
            year:null,
            half:null,
            page:1,
            pageSize:20,
            schedules:[],
            total:0,
            loading:false
        });
        this.columns1=
        [
            {
                title: '课程序号',
                dataIndex: 'scheduleId',
                key: 'scheduleId',
            },
            {
                title: '课程名称',
                dataIndex: 'lesson_name',
                key: 'lesson_name',
            },
            {
                title: '校区',
                dataIndex: 'campus',
                key: 'campus',
            },
            {
                title: '学分',
                dataIndex: 'credit',
                key: 'credit',
            },
            {
                title: '备注',
                dataIndex: 'note',
                key: 'note',
            },
            {
                title: '选课人数',
                key: 'Num',
                render: function(text, record){
                    return (
                        <p> {record.currentNum+"/"+record.maxNum}</p>
                    )
                }
            },
            {
                title: '操作',
                key: 'action',
                render: (text, record,index) =>{
                    var str = "查看";
                    return(     
                        <Space size="middle">
                            <Button type='primary' onClick={this.HandleClick.bind(this,record)}>{str} </Button>
                        </Space>
                        )
                } 
            }
        ];
//<Link to={{pathname:'/teachers/schedule',search:'?id='+str,state:{"p":str}}}></Link>
    }
    componentDidMount()
    {
        this.Get_schedules(this.state.page,this.state.pageSize,null,null);
    }
    HandleClick(item)
    {
        window.open('/teachers/schedule?id='+item.scheduleId/*,"_blank"*/);
    }
    Get_schedules( page, pageSize,year,half)
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('/api/teachers/GetSchedulesByPage',
        {
            params:{"page":page,
                    "page_size":pageSize,
                    "year":year,
                    "half":half},
            headers:{'jwt':localStorage.getItem("teacher_token")}
        })
        .then(function (response) {
            console.log(response);
            var arr = response.data.schedules;
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
            }
            console.log(arr);
            that.setState({
                schedules:arr,
                total:response.data.total,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                schedules:[],
                total:0,
                loading:false
            })
            const arr = Object.keys(error);
            console.log(arr);
            console.log(error.response.data);
        })
    }
    handlePagination(page, pageSize)
    {
        this.setState({
            page:page,
            pageSize:pageSize
        })
        this.Get_schedules(page,pageSize,this.state.year,this.state.half);
    }
    handleSelectChange(value)
    {
        //console.log(value);
        if(value == null)
        {
            this.setState({
                half:null
            })
        }
        else
        {
            this.setState({
                half:value
            })
        }
        if(value == null && this.state.year == null)
        {
            this.Get_schedules(this.state.page,this.state.pageSize,null,null);
        }
        else if(value != null && this.state.year != null)
        {
            this.setState({
                page:1
            })
            this.Get_schedules(1,this.state.pageSize, this.state.year,value);
        }
        
    }
    handleDateChange(value /*,str*/)
    {
        //console.log(value._d.getFullYear());
        if(value == null)
        {
            this.setState({
                year:null
            })
        }
        else
        {
            this.setState({
                year:value._d.getFullYear()
            })
        }
        if(value == null && this.state.half == null)
        {
            this.Get_schedules(this.state.page,this.state.pageSize,null,null);
        }
        else if(value != null && this.state.year != null)
        {
            this.setState({
                page:1
            })
            this.Get_schedules(1,this.state.pageSize, value._d.getFullYear(),this.state.half);
        }
        
    }
    showTotal()
    {
        console.log(this.state.total);
        return `共 ${this.state.total} 项记录`
    }
    render(){
        //var that = this;
        return (
            <Spin spinning={this.state.loading}>
            <div>
                <DatePicker allowClear={true} onChange={this.handleDateChange.bind(this)} picker="year" locale={locale}></DatePicker>
                <Select placeholder="上下半年" allowClear={true} style={{ width: 120 }} onChange={this.handleSelectChange.bind(this)}>
                <Option value="0">上半年</Option>
                <Option value="1">下半年</Option>
                </Select>
                <br></br>
                <br></br>
                <Table dataSource={this.state.schedules} 
                columns={this.columns1} 
                pagination={{
                    showSizeChanger:true,
                    onChange:this.handlePagination.bind(this),
                    defaultPageSize:20,
                    defaultCurrent:1,
                    pageSize:this.state.page_size,
                    current:this.state.page,
                    total:this.state.total,
                    showTotal:this.showTotal.bind(this)
                }}
                scroll={{ x: "90%" }}>
                </Table>
            </div>
            </Spin>
        );
    }
 
}

export default GetSchedules;