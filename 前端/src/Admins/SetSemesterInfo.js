
import { Button, DatePicker ,Select,Space, Input,Upload,Row,Col,Modal,Form, message, Popconfirm} from 'antd';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import { isExpire } from '../component/HandleAdminsToken';
import 'moment/locale/zh-cn';
import moment from 'moment';
import { UploadOutlined } from '@ant-design/icons';
import locale from 'antd/lib/date-picker/locale/zh_CN';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
const {Search} = Input;
const {Option} = Select;
class SetSemesterInfo extends React.Component
{
    constructor(props)
    {
        super(props);
        this.state = ({
            half:localStorage.getItem("half"),
            year:localStorage.getItem("year"),
            beginTime:null,
        });
    }
    Modify()
    {
        console.log(this.state.year,this.state.half);
        if(this.state.year==null||this.state.half==null||this.state.beginTime==null)
        {
            message.error("学期学年不能为空");
            return;
        }
        var that = this;
        axios.post('/api/admins/ModifySemester',
        {
            year:that.state.year,
            half:that.state.half,
            begintime:that.state.beginTime
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            let data = response.data;
            message.info("设置学期状态成功");
            that.props.getbasic();
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            message.error("设置学期状态失败");
        });
    }
    handleSelectChange(value)
    {
        this.setState({
            half:value
        })
    }
    handleDateChange(value)
    {
        console.log(value._d.getFullYear());
        this.setState({
            year:value._d.getFullYear()
        })
    }
    handleBeginTimeChange(value)
    {
        value._d.setHours(0,0,0,0);
        this.setState({
            beginTime: value.format("YYYY-MM-DD HH:mm:ss"),
        });
    }
    render(){  
        return (
            <div>
                <br/><br/>
                <DatePicker defaultValue={moment(localStorage.getItem("year"))} onChange={this.handleDateChange.bind(this)} picker="year" locale={locale}></DatePicker>
                <Select defaultValue={localStorage.getItem("half")} style={{ width: 120 }} onChange={this.handleSelectChange.bind(this)}>
                <Option value="0">上半年</Option>
                <Option value="1">下半年</Option>
                </Select>
                <DatePicker  onChange={this.handleBeginTimeChange.bind(this)}  placeholder='学期开始时间' locale={locale}></DatePicker>
                
                <Button onClick={this.Modify.bind(this)}>设置学期信息</Button>

            </div>
        );
    }
 
}

export default SetSemesterInfo;