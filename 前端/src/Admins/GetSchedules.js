
import { Button, Popconfirm,Table ,Select,Space, Input, Form,message, Spin, Row} from 'antd';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import { isExpire } from '../component/HandleAdminsToken';
import 'moment/locale/zh-cn';
import locale from 'antd/lib/date-picker/locale/zh_CN';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
const week=["","星期一","星期二","星期三","星期四","星期五","星期六","星期天"];
const singleORdouble=["","单周","双周",""];
const {Search} = Input;
const {Option} = Select;
class GetSchedules extends React.Component
{
    constructor(props)
    {
        super(props);
        this.state = ({
            page:1,
            pageSize:20,
            schedules:[],
            refs:[],
            total:0,
            loading:false,
            isEnd:0
        });
        this.columns1=
        [
            {
                title: '排课序号',
                key: 'scheduleId',
                render :(text,record)=>(
                    <p>{record.schedule.scheduleId}</p>
                )
            },
            {
                title: '课程名称',
                key: 'lesson_name',
                dataIndex: 'lesson_name',
            },
            {
                title: '教师',
                key: 'teacher_name',
                render: (text, record)=>{
                        return (
                            <p> {record.teacher_name}</p>
                        )
                }
            },
            {
                title: '学分',
                key: 'credit',
                render: (text, record)=>{
                    return <p> {record.credit}</p>
                }
            },
            {
                title: '上课学期',
                key: 'semester',
                render:(text,record)=>{
                    let semester;
                    if(record.schedule.half==0)
                    {
                        semester="二"
                    }
                    else if(record.schedule.half==1)
                    {
                        semester="一"
                    }
                    return <p>{record.schedule.year+record.schedule.half-1}-{record.schedule.year+record.schedule.half}学年,第{semester}学期</p>
                }
            },
            {
                title: '当前选课人数',
                key: 'currentNum',
                render: (text, record)=>{
                    return (
                        <p> {record.schedule.currentNum}</p>
                    )
                }
            },
            {
                title: '最多选课人数',
                key: 'maxNum',
                render: (text, record,index)=>{
                    if(record.isEdit)
                    {
                        return(
                            <Input onChange={this.handlemaxNumChange.bind(this,index)}
                            value={record.temp.maxNum}
                            style={{ width: 40 }}/>
                        )
                    }
                    else
                    {
                        return (
                            <p> {record.schedule.maxNum}</p>
                        )
                    }
                    
                }
            },
            {
                title: '上课地点',
                key: 'place',
                render :(text,record,index)=>{
                        return <p>{record.schedule.place}</p>
                }
            },
            {
                title: '教材',
                key: 'teachingMaterial',
                render :(text,record,index)=>{
                    if(record.isEdit)
                    {
                        return(
                            <Input onChange={this.handleteachingMaterialChange.bind(this,index)}
                            value={record.temp.note}
                            style={{ width: 40 }}/>
                        )
                    }
                    else
                    {
                        return <p>{record.schedule.teachingMaterial}</p>
                    }         
                }
            },
            {
                title: '备注',
                key: 'note',
                render :(text,record,index)=>{
                    if(record.isEdit)
                    {
                        return(
                            <Input onChange={this.handlenoteChange.bind(this,index)}
                            value={record.temp.note}
                            style={{ width: 40 }}/>
                        )
                    }
                    else
                    {
                        return <p>{record.schedule.note}</p>
                    }  
                }
            },
            {
                title: '上课时间',
                key: 'times',
                render :(text,record,index)=>{
                    if(record.isEdit)
                    {
                        let ref = React.createRef();
                        var refs = this.state.refs;
                        refs[index] = ref;
                        console.log(refs);
                        this.setState({
                            refs:refs
                        })
                        return(
                        <Form ref={ref} initialValues={{beginWeek:record.schedule.beginWeek,
                            endWeek:record.schedule.endWeek,
                            times:record.times}}>
                        <Form.Item 
                        label="起始周"
                        style={{ display: 'inline-flex',width: 'calc(55% - 4px)'}}
                        name="beginWeek" 
                        //initialValue={record.schedule.beginWeek}
                        rules={[{ required: true, message: '起始周为空' }]}
                        >
                        <Input placeholder="请入起始周"/>
                        </Form.Item>
                        <Form.Item 
                        style={{ display: 'inline-flex',width: 'calc(55% - 4px)', marginLeft: '8px' }} 
                        name="endWeek" 
                        label="结束周" 
                        //initialValue={record.schedule.endWeek}
                        rules={[{ required: true, message: '结束周为空' }]}
                        >
                        <Input placeholder="请入结束周"/>
                        </Form.Item>
                        <p>星期几&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            开始节数&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                              结束节数&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                单双周</p>
                        <Form.List name="times" /* initialValue={record.times} */>
                            {(fields, { add, remove }) => (
                            <>
                                {fields.map(({ key, name, ...restField }) => (
                                <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                                    <Form.Item
                                    {...restField}
                                    name={[name, 'dayWeek']}
                                    rules={[{ required: true, message: '缺少星期' }]}
                                    >
                                    <Select placeholder="星期几"  
                                        style={{ width: 100 }}>
                                        <Option value={1}>星期一</Option>
                                        <Option value={2}>星期二</Option>
                                        <Option value={3}>星期三</Option>
                                        <Option value={4}>星期四</Option>
                                        <Option value={5}>星期五</Option>
                                        <Option value={6}>星期六</Option>
                                        <Option value={7}>星期天</Option>
                                    </Select>
                                    </Form.Item>
                                    <Form.Item
                                    {...restField}
                                    name={[name, 'beginSection']}
                                    rules={[{ required: true, message: '缺少开始的节数' }]}
                                    >
                                    <Select placeholder="第几节开始"  
                                    
                                        style={{ width: 90 }}>
                                        <Option value={1}>1</Option>
                                        <Option value={2}>2</Option>
                                        <Option value={3}>3</Option>
                                        <Option value={4}>4</Option>
                                        <Option value={5}>5</Option>
                                        <Option value={6}>6</Option>
                                        <Option value={7}>7</Option>
                                        <Option value={8}>8</Option>
                                        <Option value={9}>9</Option>
                                        <Option value={10}>10</Option>
                                        <Option value={11}>11</Option>
                                        <Option value={12}>12</Option>
                                    </Select>
                                    </Form.Item>
                                    <Form.Item
                                    {...restField}
                                    name={[name, 'endSection']}
                                    rules={[{ required: true, message: '缺少结束的节数' }]}
                                    >
                                    <Select placeholder="第几节结束"  
                                        style={{ width: 90 }}>
                                        <Option value={1}>1</Option>
                                        <Option value={2}>2</Option>
                                        <Option value={3}>3</Option>
                                        <Option value={4}>4</Option>
                                        <Option value={5}>5</Option>
                                        <Option value={6}>6</Option>
                                        <Option value={7}>7</Option>
                                        <Option value={8}>8</Option>
                                        <Option value={9}>9</Option>
                                        <Option value={10}>10</Option>
                                        <Option value={11}>11</Option>
                                        <Option value={12}>12</Option>
                                    </Select>
                                    </Form.Item>
                                    <Form.Item
                                    {...restField}
                                    name={[name, 'singleOrDouble']}
                                    rules={[{ required: true, message: '缺少单双周' }]}
                                    >
                                    <Select  placeholder="单双周"  
                                        style={{ width: 100 }}>
                                        <Option value={3}>全</Option>
                                        <Option value={1}>单周</Option>
                                        <Option value={2}>双周</Option>
                                    </Select>
                                    </Form.Item>
                                    <MinusCircleOutlined onClick={() => remove(name)} />
                                </Space>
                                ))}
                                <Form.Item>
                                <Button type="dashed" onClick={() => add()} block icon={<PlusOutlined />}>
                                    增加上课时间
                                </Button>
                                </Form.Item>
                            </>
                            )}
                        </Form.List>
                        </Form>)
                    }
                    else
                    {
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
                            <p> {"["+record.schedule.beginWeek+"-"+record.schedule.endWeek+"]"+str}</p>
                        )
                        
                    }
                    }
            },
            {
                title: '操作',
                key: 'action',
                render: (text, record,index) =>{
                    if(record.isEdit)
                    {
                        return(     
                            <Space size="middle">
                                <Button type='primary' onClick={this.HandleYesClick.bind(this,index)}>确定 
                                </Button>
                                <Button type='primary' onClick={this.HandleCancelClick.bind(this,index)}>取消 
                                </Button>
                            </Space>
                        )
                    }
                    else
                    {
                        return(     
                            <Space size="middle">
                                <Button type='primary' onClick={this.HandleEditClick.bind(this,index)}>编辑 
                                </Button>
                            </Space>
                        )
                    }
                    
                } 
            }
        ];
    }

    handlemaxNumChange(s_index,event)
    {
        var schedules = JSON.parse(JSON.stringify(this.state.schedules));
        schedules[s_index].temp.maxNum = event.target.value;
        this.setState({schedules: schedules});
    }
    handlePlaceChange(s_index,event)
    {
        var schedules = JSON.parse(JSON.stringify(this.state.schedules));
        schedules[s_index].temp.place = event.target.value;
        this.setState({schedules: schedules});
    }
    handlenoteChange(s_index,event)
    {
        var schedules = JSON.parse(JSON.stringify(this.state.schedules));
        schedules[s_index].temp.note = event.target.value;
        this.setState({schedules: schedules});
    }
    handleteachingMaterialChange(s_index,event)
    {
        var schedules = JSON.parse(JSON.stringify(this.state.schedules));
        schedules[s_index].temp.teachingMaterial = event.target.value;
        this.setState({schedules: schedules});
    }
    HandleYesClick(s_index)
    {
        
        var _schedules = JSON.parse(JSON.stringify(this.state.schedules));
        if(_schedules[s_index].temp.place == null  || _schedules[s_index].temp.maxNum ==null
            || this.state.refs[s_index].current.getFieldsValue().times == null 
            || this.state.refs[s_index].current.getFieldsValue().beginWeek == null
            || this.state.refs[s_index].current.getFieldsValue().endWeek == null) 
        {
            message.error("除备注，教材外不能为空");
            return;
        }
        else if(_schedules[s_index].temp.maxNum < _schedules[s_index].schedule.maxNum)
        {
            message.error("人数上限不能减少");
            return;
        }
        else if(this.state.refs[s_index].current.getFieldsValue().beginWeek > this.state.refs[s_index].current.getFieldsValue().endWeek)
        {
            message.error("开始周不能在结束周之后");
            return;
        }
        var that = this;
        var schedules = _schedules.map(function(item,index,array){
            if(index == s_index)
            {
                item.isEdit = false;
                item.schedule.maxNum = item.temp.maxNum ;
                item.schedule.place = item.temp.place ;
                item.schedule.teachingMaterial = item.temp.teachingMaterial ;
                item.times = that.state.refs[s_index].current.getFieldsValue().times;
                item.schedule.beginWeek = that.state.refs[s_index].current.getFieldsValue().beginWeek ;
                item.schedule.endWeek = that.state.refs[s_index].current.getFieldsValue().endWeek ;
                item.schedule.note = item.temp.note ;
            }  
            return item;
        })
        console.log(schedules[s_index].temp);
        console.log(schedules[s_index].temp.note);
        axios.post('/api/admins/ModifySchedule',
        {
            "schedule_id":schedules[s_index].schedule.scheduleId,
            "place":schedules[s_index].temp.place,
            "maxNum":schedules[s_index].temp.maxNum,
            "times":that.state.refs[s_index].current.getFieldsValue().times,
            "teachingMaterial":schedules[s_index].temp.teachingMaterial,
            "note":schedules[s_index].temp.note,
            "begin_week": that.state.refs[s_index].current.getFieldsValue().beginWeek,
            "end_week":that.state.refs[s_index].current.getFieldsValue().endWeek
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            message.info("成功修改课程信息");
            console.log(response);
            console.log(response.data);
            that.setState({
                schedules:schedules
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                window.location.href="/admins/log"
            message.error("修改成绩失败");
            console.log(error);
        })
        
    }
    HandleCancelClick(s_index)
    {
        var schedules = JSON.parse(JSON.stringify(this.state.schedules));
        console.log(this.state.refs);
        console.log(this.state.refs[s_index]);
        console.log(this.state.refs[s_index].current.getFieldsValue());
        this.state.refs[s_index].current.setFieldsValue({
            times:schedules[s_index].times,
            beginWeek:schedules[s_index].beginWeek,
            endWeek:schedules[s_index].endWeek,
        });
        schedules[s_index].isEdit = false;
        schedules[s_index].temp.place = schedules[s_index].schedule.place;
        schedules[s_index].temp.maxNum = schedules[s_index].schedule.maxNum;
        //schedules[s_index].temp.requirements = schedules[s_index].requirements;
        schedules[s_index].temp.note = schedules[s_index].schedule.note;
        schedules[s_index].temp.teachingMaterial = schedules[s_index].schedule.teachingMaterial;
        this.setState({
            schedules:schedules
        })
    }
    HandleEditClick(s_index)
    {
        let schedules = this.state.schedules;
        console.log(s_index);
        console.log(this.state.schedules);
        schedules.forEach(function(item, index, arr) {
            if(index == s_index)
                item.isEdit = true;
        });
        console.log(schedules);
        this.setState({
            schedules:JSON.parse(JSON.stringify(schedules)) 
        })  
    }
    componentDidMount()
    {
        this.Get_isEnd();
        this.Get_schedules(this.state.page,this.state.pageSize);
    }
    Get_isEnd()
    {
        var that = this;
        axios.get('api/admins/EndSchedules',
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            that.setState({
                isEnd:response.data
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            const arr = Object.keys(error);
            console.log(arr);
        })
    }
    Get_schedules( page, pageSize)
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('api/admins/GetSchedules',
        {
            params:{"page":page,
                    "page_size":pageSize},
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            var arr = response.data.schedules;
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
                arr[i].isEdit = false;
                arr[i].temp = { place:arr[i].schedule.place,
                                maxNum:arr[i].schedule.maxNum,
                                times:arr[i].times,
                                note:arr[i].schedule.note,
                                teachingMaterial:arr[i].schedule.teachingMaterial};
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
    onSearch(value)
    {
        console.log(value=="");
        console.log(this.state.page);
        console.log(this.state.pageSize);
        if(value == ""||value == null)
        {
            this.Get_schedules(this.state.page,this.state.pageSize);
            return;
        }
        this.setState({
            page:1
        })
        var that = this;
        axios.get('/api/admins/QuerySchedule',
        {
            params:{"schedule_id":value},
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
                schedules:new_arr,
                total:1
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                schedules:[],
                total:0
            })
            const arr = Object.keys(error);
            console.log(arr);
            console.log(error.response.data);
        });
    }
    HandleEndClick()
    {
        var that = this;
        this.setState({
            loading: true
        })
        axios.post('/api/admins/EndSchedules',
        {

        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            message.info("结课成功");
            that.setState({
                isEnd:1,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            else
            {
                message.error("结课时间过早");
            }  
            that.setState({
                loading:false
            })
        });
    }
    showTotal()
    {
        //console.log(total);
        return `共 ${this.state.total} 项记录`
    }
    render(){
        //var that = this;
        return (
            <Spin spinning={this.state.loading}>
            <div>
                <Row>
                <Search type="number" allowClear={true} placeholder="输入排课序号" onSearch={this.onSearch.bind(this)} style={{ width: 200 }} />
                &nbsp;&nbsp;&nbsp;
                <Popconfirm okText="是" cancelText="否" disabled={this.state.isEnd==1} title="确认要结课吗" onConfirm={this.HandleEndClick.bind(this)}> 
                    <Button disabled={this.state.isEnd==1} type='primary'> 本学期结课
                    </Button>
                </Popconfirm>
                </Row>
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
                scroll={{ x: "100%" }}>
                </Table>
            </div>
            </Spin>
        );
    }
 
}

export default GetSchedules;