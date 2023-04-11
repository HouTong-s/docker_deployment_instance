import moment from 'moment';
import { Button, DatePicker,Table ,Select, Spin} from 'antd';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import TimeTable from '../component/TimeTable';
import { isExpire } from '../component/HandleTokenExpire';
import 'moment/locale/zh-cn';
import locale from 'antd/lib/date-picker/locale/zh_CN';
const {Option} = Select;
const week=["","星期一","星期二","星期三","星期四","星期五","星期六","星期天"];
const singleORdouble=["","单周","双周",""];
class GetTimeTable extends React.Component
{
    constructor(props)
    {
        super(props);
        this.state = ({
            visible:false,
            year:localStorage.getItem("year"),
            half:localStorage.getItem("half"),
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
                title: '教师',
                dataIndex: 'teacher_name',
                key: 'teacher_name',
            },
            {
                title: '地点',
                dataIndex: 'place',
                key: 'place',
            },
            {
                title: '时间',
                key: 'times',
                render: function(text, record,index){
                    var arr = new Array();
                    record.times.forEach(element => {
                        var temp= element.singleOrDouble==3?"":("("+singleORdouble[element.singleOrDouble]+")");
                        arr.push(week[element.dayWeek]+" "+element.beginSection+"-"+element.endSection+"节"+temp+"  ");
                    });
                    var str = "";
                    arr.forEach(element => {
                        str+=element;
                    });
                    return (
                        <p> {"["+record.beginWeek+"-"+record.endWeek+"]"+str}</p>
                    )
                }
                /* (text, record) => (<p>
                    {"["+record.beginWeek+"-"+record.endWeek+"]"}
                    {record.times.forEach(element => {
                        week[element.dayWeek]+element.BeginSection+"-"+element.BeginSection+"节"+singleORdouble[element.singleOrDouble]
                    }).toString()}
                    </p>
                )*/
            },
        ];

    }
    componentDidMount()
    {
        this.GetTimeTables(localStorage.getItem("year"),localStorage.getItem("half"));
    }
    GetTimeTables(x,y)
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('/api/students/GetTimetable',
        {
            params:{"year":parseInt(x),
                    "half":parseInt(y)},
            headers:{'jwt':localStorage.getItem("token")}
        })
        .then(function (response) {
            console.log(response);
            var arr = response.data;
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
            }
            console.log(arr);
            that.setState({
                time_table:arr,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                time_table:[],
                loading:false
            })
            const arr = Object.keys(error);
            console.log(arr);
            console.log(error.response.data);
        })
    }
    handleSelectChange(value)
    {
        console.log(value);
        if(value == null)
            return;
        
        this.setState({
            half:value
        })
        if(this.state.year!=null)
        {
            console.log("get");
            this.GetTimeTables(this.state.year,value);
        }
    }
    handleDateChange(value,str)
    {
        console.log(value._d.getFullYear());
        if(value == null)
            return;
        
        this.setState({
            year:value._d.getFullYear()
        })
        console.log(this.state.half);
        if(this.state.half!=null)
        {
            console.log("get");
            this.GetTimeTables(value._d.getFullYear(),this.state.half);
        }
    }
    render(){
        return (
            <Spin spinning={this.state.loading}>
            <div>
                <DatePicker defaultValue={moment(localStorage.getItem("year"))} onChange={this.handleDateChange.bind(this)} picker="year" locale={locale}></DatePicker>
                <Select defaultValue={localStorage.getItem("half")} style={{ width: 120 }} onChange={this.handleSelectChange.bind(this)}>
                <Option value="0">上半年</Option>
                <Option value="1">下半年</Option>
                </Select>
                <br></br>
                <br></br>
                <TimeTable schedules={this.state.time_table}></TimeTable>
                <br></br>
                <br></br>
            <Table dataSource={this.state.time_table} columns={this.columns1} scroll={{ x: "100%" }}></Table>
            </div>
            </Spin>
        );
    }
 
}
export default GetTimeTable;