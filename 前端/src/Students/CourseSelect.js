
import { Button, DatePicker,Table ,Space, Modal, Col,Select, message, Spin} from 'antd';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import TimeTable from '../component/TimeTable';
import { IsConflict1 } from '../component/TimeConfilctJudgement';
import { isExpire } from '../component/HandleTokenExpire';
const {Option} = Select;
const week=["","星期一","星期二","星期三","星期四","星期五","星期六","星期天"];
const singleORdouble=["","单周","双周",""];
const select_status=["","新修选课","重修选课"]
class CourseSelect extends React.Component
{
    constructor(props)
    {
        super(props);
        this.state = ({
            visible:false,
            can_select:true,
            optional_lessons:[],
            current_table:[],
            lessons:[],
            selected_lessons:[],
            types:[],
            origin_table:[],
            add_task:-1,
            delete_task:-1,
            add_num:0,
            delete_num:0,
            fail_num:0,
            lessons_changed:false,
            type:null,
            new_list:[],
            loading:false
        });
        this.columnsAll=
        [
            {
                title: '课程号',
                dataIndex: 'lessonId',
                key: 'lessonId',
            },
            {
                title: '课程名称',
                dataIndex: 'lessonName',
                key: 'lessonName',
            },
            {
                title: '课程类型',
                dataIndex: 'type',
                key: 'type',
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
                title: '操作',
                key: 'action',
                render: (text, record) => (
                    
                <Space size="middle">
                    <Button type='primary' onClick={this.handleOptionClick.bind(this,record)}>加入备选 </Button>
                </Space>
                ),
            },
        ];
        this.columnsSelect = 
        [
            {
                title: '课程号',
                dataIndex: 'lessonId',
                key: 'lessonId',
            },
            {
                title: '课程名称',
                dataIndex: 'lessonName',
                key: 'lessonName',
            },
            {
                title: '课程类型',
                dataIndex: 'type',
                key: 'type',
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
                title: '操作',
                key: 'action',
                render: (text, record) => (
                    
                <Space size="middle">
                    <Button type='primary' onClick={this.handleCheckClick.bind(this,record)}>查看 </Button>
                </Space>
                ),
            },
        ];
        this.columnSchedules=
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
                title: '课程类型',
                dataIndex: 'type',
                key: 'type',
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
                title: '时间',
                key: 'times',
                render: (text, record,index)=>{
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
            },
            {
                title: '选课人数',
                key: 'Num',
                render: function(text, record,index){
                    return (
                        <p> {record.currentNum+"/"+record.maxNum}</p>
                    )
                }
            },
            {
                title: '操作',
                key: 'action',
                render: (text, record) => {
                    if(record.is_selected)
                    {
                        return  <p> 已选择 </p>
                    }
                    else
                    {
                        return(
                            <Space size="middle">
                                <Button type='primary' onClick={this.handleSelectClick.bind(this,record)}>选择 </Button>
                            </Space>
                            )
                    }  
                },
            },
        ];
        this.columnsCurrent=
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
            },
            {
                title: '操作',
                key: 'action',
                render: (text, record) => {
                    console.log(record.select_type);
                    console.log(record.selectstatus);
                    console.log(select_status[record.selectstatus]);
                    return (
                    <Space size="middle">
                    <Button disabled={record.select_type!=null && record.selectstatus!=0&&record.select_type!=select_status[record.selectstatus]} danger type='primary' onClick={this.handleDeleteClick.bind(this,record)}>删除 </Button>
                        </Space>)
                },
            },
        ];
    }
    componentDidMount()
    {
        this.Getlessons();           
    }
    Getlessons()
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('/api/students/GetLessons',
        {
            headers:{'jwt':localStorage.getItem("token")}
        })
        .then(function (response) {
            var arr = response.data.lessons;
            
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
            }
            var types = [];
            arr.forEach(function(item,index,array){
                if(types.indexOf(item.type)==-1)//不存在
                {
                    types.push(item.type);
                }
            })
            that.setState({
                types:types,
                lessons:arr,
                selected_lessons:arr,
                begin_time:response.data.begin_time,
                end_time:response.data.end_time,
                select_type:response.data.select_type,
                loading:false
            })
            that.GetTimetables();
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            const arr = Object.keys(error);
            that.setState({
                loading:false
            })
            if(error.response.status==404)
            {
                that.setState({
                    lessons:[],                  
                })
            }
            else if(error.response.status==400)
            {
                that.setState({
                    can_select:false
                })
            }
        })
    }
    handleCheckClick(item)
    {
        var that = this;
        var temp = item.schedules
        this.setState({
            schedules:temp,
            visible:true,
        })
    }
    handleOptionClick(item)
    {
        console.log(item);
        var that = this;
        axios.get('/api/students/GetSchedules/'+item.lessonId,
        {
            headers:{'jwt':localStorage.getItem("token")}
        })
        .then(function (response) {
            console.log(response);
            var arr = response.data;
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
                arr[i].is_selected = false;
            }
            console.log(arr);
            var temp = that.state.optional_lessons;
            item.schedules = arr;
            temp = temp.concat(item);

            temp.forEach(function(item, index, arr) {
                item.key = index;
            });
            that.setState({
                optional_lessons:temp
            })

            var lessons = that.state.lessons;
            
            lessons.forEach(function(item1, index, arr) {
                if(item1.lessonId==item.lessonId ) {
                    arr.splice(index, 1);
                }
            });
            
            var arr = new Array();
            arr = JSON.parse(JSON.stringify(lessons));
            arr.forEach(function(item, index, arr) {
                item.key = index
            });
            console.log(arr);
            if(arr.length == 0)
            {
                that.setState({
                    lessons:[],
                    lessons_changed:true
                });
            }
            else
            {
                that.setState({
                    lessons:arr,
                    lessons_changed:true
                });
            }
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            const arr = Object.keys(error);
            console.log(arr);
        })
    }
    handleSelectClick(item)
    {
        var optional_lessons = this.state.optional_lessons;
        var new_schedule=[];
        optional_lessons.forEach(function(item1,index1,array1){
            if(item1.lessonId == item.lessonId)
            {
                item1.schedules.forEach(function(item2,index2,array2){
                    item2.is_selected = false;
                    if(item2.scheduleId==item.scheduleId)
                        item2.is_selected = true;
                    new_schedule.push(item2)
                })

            }
        })
        this.setState({
            optional_lessons:optional_lessons
        })
        this.setState({
            schedules:JSON.parse(JSON.stringify(new_schedule)) 
        })
        console.log(new_schedule);
        
        var CurrentTables = this.state.current_table;
        var is_repeat = false;
        CurrentTables.forEach(function(item1,index1,array1){
            if(item1.lessonId == item.lessonId)
            {
                is_repeat = true;
            }
        })
        console.log(is_repeat);
        if(is_repeat)
        {
            const temps = [...CurrentTables];
            this.setState({
                current_table:temps.map(function(item1,index1,array1){
                    if(item1.lessonId == item.lessonId)
                    {
                        return item;
                    }       
                    else
                    {
                        return item1;
                    }
                })
            })
        }
        else
        {
            CurrentTables= CurrentTables.concat(item);
            CurrentTables.forEach(function(item1, index1, arr1) {
                item1.key = index1;
            });
            var arr = new Array();
            arr = JSON.parse(JSON.stringify(CurrentTables));
            
            //深拷贝，防止出错
            this.setState({
                current_table:arr
            });
        }     
    }
    handleDeleteClick(element)
    {
        var CurrentTables = this.state.current_table;
        var optional_lessons = this.state.optional_lessons;
        optional_lessons.forEach(function(item, index, arr) {
            if(item.lessonId==element.lessonId ) {
                item.schedules.forEach(function(item1, index1, arr1) {
                    item1.is_selected = false;
                })
            }
        });
        this.setState({
            optional_lessons:optional_lessons
        })
        CurrentTables.forEach(function(item, index, arr) {
            if(item.scheduleId==element.scheduleId ) {
                arr.splice(index, 1);
            }
        });
        CurrentTables.forEach(function(item, index, arr) {
            item.key = index
        });
        
        var arr = new Array();
        arr = JSON.parse(JSON.stringify(CurrentTables));
        //因为删除了一项，所以需要深拷贝
        if(arr.length == 0)
        {
            this.setState({
                current_table:[]
            });
        }
        else
        {
            this.setState({
                current_table:arr
            });
        }
        
    }
    Judge_Has_Conflict(array)
    {
        console.log(array);
        var len = array.length;
        if(len<=1)
        {
            return false;
        }
        for(var i=0;i<len-1;i++)
        {
            console.log(array[i]);
            console.log(array.slice(i+1,len))
            if(IsConflict1(array[i],array.slice(i+1,len)))
            {
                return true;
            }
        }
        return false;
    }
    SaveSchedules()
    {
        console.log("1");
        var current = this.state.current_table;
        if(this.Judge_Has_Conflict(current))
        {
            message.error("选的课程中有时间冲突");
            return;
        }
        var origin = this.state.origin_table;
        var new_list = [];
        var delete_list = [];
        
        current.forEach(function(item,index,array){
            var is_new = true;
            origin.forEach(function(item1,index1,array1){
                if(item1.scheduleId == item.scheduleId)
                    is_new = false;
            })
            if(is_new)
            {
                new_list.push(item.scheduleId);
            }
        })
        this.setState({
            new_list:new_list
        })
        origin.forEach(function(item,index,array){
            var is_old = true;
            current.forEach(function(item1,index1,array1){
                if(item1.scheduleId == item.scheduleId)
                    is_old = false;
            })
            if(is_old)
            {
                delete_list.push(item.scheduleId);
            }
        })
        console.log("2");
        if(new_list.length==0&&delete_list.length==0)
        //没有更改的项目
        {
            message.info("您的课表没有变动");
            return;
        }
        var that = this;
        this.setState({
            add_task:new_list.length,
            delete_task:delete_list.length,
            add_num:0,
            delete_num:0,
            fail_num:0
        })
        if(delete_list.length>0)
        {
            console.log("退课");
            delete_list.forEach(function(item,index,array){
                axios.post('/api/students/CancelSchedules',
                {
                    schedule_id:item
                },
                {
                    headers:{'jwt':localStorage.getItem("token")}
                })
                .then(function (response) {
                    console.log(response);
                    if(response.data.code == 204)
                    {
                        that.setState((state) => ({
                            delete_num: state.delete_num +1
                          }));
                    }
                    else if(response.data.code == 403)
                    {
                        message.error(response.data.detail)
                    }  
                })
                .catch(function (error) {
                    if(isExpire(error))
                        that.props.redirect();
                    const arr = Object.keys(error);
                    that.setState((state) => ({
                        fail_num: state.fail_num +1
                      }));
                    console.log(error);
                })
            })
        }   
        console.log("3");
    }
    GetTimetables()
    {
        var that = this;
        
        axios.get('/api/students/GetTimetable',
        {
            params:{"year":localStorage.getItem("year"),
                    "half":localStorage.getItem("half")},
            headers:{'jwt':localStorage.getItem("token")}
        })
        .then(function (response) {
            console.log(response);
            var arr = response.data;
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
                arr[i].select_type = that.state.select_type;
            }
            that.setState({
                current_table:arr,
                origin_table:JSON.parse(JSON.stringify(arr))
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                current_table:null
            })
            const arr = Object.keys(error);
            console.log(error.response);
        })
    }
    cancelModal()
    {
        this.setState({
            visible:false
        })
    }
    handleSelectChange(value)
    {
        console.log(value);
        if(value == null)
        {
            var lessons = this.state.lessons
            this.setState({
                selected_lessons:JSON.parse(JSON.stringify(lessons)),
                type:value
            })
            return;
        }
        this.setState({
            type:value
        })
        var temp = new Array();
        this.state.lessons.forEach(function(item,index,array){
            if(item.type == value)
            {
                temp.push(item);
            }
        })
        temp.forEach(function(item,index,array){
            item.key=index
        })
        this.setState({
            selected_lessons:temp
        })
    }
    render(){
        if(!this.state.can_select)
        {
            return (<p>不在选课时间内</p>)
        }
        if(this.state.lessons_changed)
        {
            this.handleSelectChange(this.state.type);
            this.setState({
                lessons_changed:false
            })
        }
        if(this.state.delete_num == this.state.delete_task)
        //先执行退课再选课(不然会出现选不到的情况)
        {
            this.setState({
                delete_num:-1
            })
            var new_list =this.state.new_list;
            var that = this;
            if(new_list.length==0)
            {
                this.setState({
                    add_task:1,
                    add_num:1
                })
            }
            else
            {
                new_list.forEach(function(item,index,array){         
                    axios.post('/api/students/SelectSchedules',
                    {
                        schedule_id:item
                    },
                    {
                        headers:{'jwt':localStorage.getItem("token")}
                    })
                    .then(function (response) {
                        if(response.data.code==204)
                        {
                            that.setState((state) => ({
                                add_num: state.add_num +1
                              }));
                        }
                        else if(response.data.code == 409)
                        {
                            message.error(response.data.detail)
                        }  
                    })
                    .catch(function (error) {
                        if(isExpire(error))
                            that.props.redirect();
                        const arr = Object.keys(error);
                        that.setState((state) => ({
                            fail_num: state.fail_num +1
                          }));
                        console.log(error.response);
                    })
                })
            }
        }
        if(this.state.add_task == this.state.add_num && this.state.add_task!=0)
        {
            this.setState({
                visible:false,
                can_select:true,
                optional_lessons:[],
                current_table:[],
                lessons:[],
                selected_lessons:[],
                types:[],
                origin_table:[],
                add_task:-1,
                delete_task:-1,
                add_num:0,
                delete_num:0,
                fail_num:0,
                lessons_changed:false,
                type:null,
                new_list:[]
            });
            message.info('选课成功');
            this.Getlessons();
        }
        else if(this.fail_num>1)
        {
            this.setState({
                visible:false,
                can_select:true,
                optional_lessons:[],
                current_table:[],
                lessons:[],
                selected_lessons:[],
                types:[],
                origin_table:[],
                add_task:-1,
                delete_task:-1,
                add_num:0,
                delete_num:0,
                fail_num:0,
                lessons_changed:false,
                type:null,
                new_list:[]
            });
            message.error('选课出现错误');
            this.Getlessons();
            this.GetTimetables();   
        }
        let dt1 = null;
        let dt2 = null;
        if(this.state.begin_time!=null)
            dt1 = this.state.begin_time.split("T");
        if(this.state.end_time!=null)
            dt2 = this.state.end_time.split("T");
        return (

            <div>
            <Spin spinning={this.state.loading}>
                <p>选课时间：{dt1?dt1[0]:null} {dt1?dt1[1]:null}  到  {dt2?dt2[0]:null} {dt2?dt2[1]:null}</p>
                <p>   选课类型：{this.state.select_type}</p>
                <br></br>
                <br></br>
                <p>所有待选课程：</p>
                <Select allowClear={true} 
                value={this.state.type} 
                placeholder={ "选择课程种类" }  
                style={{ width: 130 }} 
                onChange={this.handleSelectChange.bind(this)}>
                    {this.state.types.map(function(item,index,array){
                        return (<Option value={item}>{item}</Option>)
                    })}
                </Select>
                <br></br>
            
            <Table dataSource={this.state.selected_lessons} columns={this.columnsAll} scroll={{ x: "100%" ,y:""}}></Table>
            </Spin>
            <br></br>
            <br></br>
            <p>个人备选课程：</p>
            <Table dataSource={this.state.optional_lessons} columns={this.columnsSelect}></Table>
            <br></br>
            <br></br>
            <Col span={3} offset={20}><Button  onClick={this.SaveSchedules.bind(this)}>保存课表</Button> </Col>
            <p>当前课表：</p>
            <TimeTable schedules={this.state.current_table}></TimeTable>
            <Modal  width={800}
            visible={this.state.visible} 
            onOk={this.cancelModal.bind(this)} 
            onCancel={this.cancelModal.bind(this)}
            bodyStyle={{height: '300px', overflowY: 'auto',overflowX:'auto'}}>
            <Table dataSource={this.state.schedules} columns={this.columnSchedules}></Table>
            </Modal>
            <br></br>
            <br></br>
            <br></br>
            <p>课表详细信息：</p>
            <Table dataSource={this.state.current_table} columns={this.columnsCurrent}></Table>
            </div>
        );
    }
 
}

export default CourseSelect;
