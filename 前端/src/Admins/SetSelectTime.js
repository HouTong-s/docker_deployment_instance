
import { Button, DatePicker ,Select,Space, Input,Spin,Row,Col,Modal,Form, message, Popconfirm} from 'antd';
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
class SetSelectTime extends React.Component
{
    constructor(props)
    {
        super(props);
        this.state = ({
            status:null,
            beginTime:null,
            endTime:null,
            showstatus:null,
            showbeginTime:null,
            showendTime:null,
            loading:false
        });
    }
    componentDidMount()
    {
        this.getinfo();
    }
    getinfo()
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('/api/admins/CourseSelectStatus',
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            let data = response.data;
            that.setState({
                showstatus:data.status,
                showbeginTime:data.beginTime,
                showendTime:data.endTime,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                showstatus:0,
                showbeginTime:null,
                showendTime:null,
                loading:false
            })
        });
    }
    StatusChange(value)
    {
        this.setState({
            status:value
        })
    }
    Modify()
    {
        let begin,end;
        if(this.state.status==null)
        {
            message.error("选课状态不能为空");
            return;
        }
        else if(this.state.status==0)
        {
            begin = null;
            end = null
        }
        else if(this.state.beginTime==null||this.state.endTime==null)
        {
            message.error("选课时间不能为空");
            return;
        }
        var that = this;
        axios.post('/api/admins/CourseSelectStatus',
        {
            selectBeginTime:that.state.beginTime,
            selectEndTime:that.state.endTime,
            status:that.state.status
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            let data = response.data;
            message.info("设置选课状态成功");
            that.getinfo();
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            message.error("设置选课状态失败");
        });
    }
    handleDateChange(value)
    {
        value[0]._d.setHours(8,0,0,0);
        value[1]._d.setHours(20,0,0,0);
        //moment.format("YYYYMMDD")
        console.log(value);
        console.log(value[0].format("YYYY-MM-DD HH:mm:ss"));
        /*
        console.log(value[0]._d.toString());
        console.log(value[0]._d.toTimeString());
        console.log(value[0]._d.toUTCString());
        console.log(value[1]._d.toDateString());
        */
        this.setState({
            beginTime: value[0].format("YYYY-MM-DD HH:mm:ss"),
            endTime: value[1].format("YYYY-MM-DD HH:mm:ss")
        });
        
    }
    render(){
        //var that = this;
        var str="";
        if(this.state.showstatus == 0)
        {
            str= "未开放选课";
        }
        else if(this.state.showstatus == 1)
        {
            str= "新修选课";
        }
        else if(this.state.showstatus == 2)
        {
            str= "重修选课";
        }
        /*
        var bt=[];
        var et=[];
        if(this.state.beginTime!=null)
        {
            let arr = this.state.beginTime.split("T");
            bt.push(arr[0]);
        }
        if(this.state.endTime!=null)
        {

        }
        */
        let arr =[];
        if(this.state.showbeginTime!=null)
        {
            arr = this.state.showbeginTime.split("T");
            str+="  开放时间：  "+arr[0]+" "+arr[1];
        }
        let brr =[];
        if(this.state.showendTime!=null)
        {
            brr = this.state.showendTime.split("T");
            str+="  到  "+brr[0]+" "+brr[1];
        }
        
        return (
            <Spin spinning={this.state.loading}>
            <div>
                <b>当前选课状态 </b>
                <br/><br/>
                <p>{str}</p>
                <Select style={{"width":"120px"}} onChange={this.StatusChange.bind(this)} placeholder="选课状态">
                <Option value={0}> 不开放选课</Option>
                <Option value={1}> 新修选课</Option>
                <Option value={2}> 重修选课</Option>
                </Select>
                {this.state.status==0?null:<DatePicker.RangePicker onChange={this.handleDateChange.bind(this)} locale={locale}></DatePicker.RangePicker>}
                
                <Button onClick={this.Modify.bind(this)}>设置选课状态</Button>

            </div>
            </Spin>
        );
    }
 
}

export default SetSelectTime;