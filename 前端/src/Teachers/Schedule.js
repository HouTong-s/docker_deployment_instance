
//import { Table , Row, Col} from 'antd';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import { isExpire } from '../component/HandleTokenExpire';
import { Col, Row, Table,Space,Button,Upload ,message,Spin, Input,Form,Select} from 'antd';
import { UploadOutlined } from '@ant-design/icons';
const {Option} = Select;
class OneSchedule extends React.Component
{
    constructor(props)
    {
        super(props);
        
        // eslint-disable-next-line react/prop-types
        console.log(window.location.search);
        var arr=window.location.search.split("?")[1].split("=");
        var len=arr.length;
        this.state = ({
            id:arr[len-1],
            students:[],
            isOver:null,
            year:null,
            half:null,
            currentNum:null,
            maxNum:null,
            place:null,
            scheduleId:null,
            campus:null,
            LessonName:null,
            canImportGrade:null,
            loading:false
        });
        this.columns1=
        [
            {
                title: '学号',
                key: 'studentId',
                dataIndex: 'studentId',
            },
            {
                title: '姓名',
                key: 'studentName',
                dataIndex: 'studentName',
            },
            {
                title: '专业',
                key: 'department',
                dataIndex: 'department',
            },
            {
                title: '分数',
                key: 'score',
                render:(text,record,index)=>{
                    if(record.isEdit)
                    {
                        return(
                            <Input onChange={this.handleScoreChange.bind(this,index)}
                            value={record.temp.score}
                            type="number"
                            style={{ width: 80 }}/>
                            )
                    }
                    else
                    {
                        return <p>{record.score}</p>
                    }
                }
            },
            {
                title: '成绩状态',
                key: 'gradeStatus',
                render:(text,record,index)=>{
                    //style={{ width: 120 }}
                    if(record.isEdit)
                    {
                        return(
                            <Select onChange={this.handleSelectChange.bind(this,index)} 
                            value={record.temp.gradeStatus}
                            style={{ width: 80 }}>
                            <Option value="正常">正常</Option>
                            <Option value="旷考">旷考</Option>
                            </Select>
                        )
                    }
                    else
                    {
                        return <p>{record.gradeStatus}</p>
                    }
                }
            },
            {
                title: '操作',
                key: 'Action',
                render:(text,record,index)=>{
                    if(record.isEdit)
                    {
                        return<div>
                                <Button type='primary' onClick={this.HandleYesClick.bind(this,index)}>确定 
                                </Button>
                                <Button type='primary' onClick={this.HandleCancelClick.bind(this,index)}>取消 
                                </Button>
                                </div>;
                    }
                    else
                    {
                        return <Button disabled={this.state.isOver == 1 || this.state.canImportGrade == 0 || record.score!=null} type='primary' onClick={this.HandleChangeClick.bind(this,index)}>登入成绩 </Button>;
                    }
                
                }
            }
        ];
    }
    //s_index为数组的下标
    handleScoreChange(s_index,event)
    {
        var students = JSON.parse(JSON.stringify(this.state.students));
        students[s_index].temp.score = event.target.value;
        this.setState({students: students});
    }
    handleSelectChange(s_index,value)
    {
        var students = JSON.parse(JSON.stringify(this.state.students));
        students[s_index].temp.gradeStatus = value;
        this.setState({students: students});
    }
    HandleYesClick(s_index)
    {
        var _students = JSON.parse(JSON.stringify(this.state.students));
        var students = _students.map(function(item,index,array){
            if(index == s_index)
            {
                item.isEdit = false;
                item.score = item.temp.score;
                item.gradeStatus = item.temp.gradeStatus;
            }  
            return item;
        })
        if(students[s_index].temp.score == null  || students[s_index].temp.gradeStatus ==null)
        {
            message.error("分数和成绩状态不能为空");
            return;
        }
        else if( students[s_index].temp.score<0)
        {
            message.error("成绩分数不能为负数");
            return;
        }
        var that = this;
        this.setState({
            loading:true
        })
        axios.post('/api/teachers/RegeisterScore',
        {
            "student_id":students[s_index].studentId,
            "schedule_id":that.state.id,
            "score":students[s_index].score,
            "status":students[s_index].gradeStatus
        },
        {
            headers:{'jwt':localStorage.getItem("teacher_token")}
        })
        .then(function (response) {
            message.info("成功登入成绩");
            console.log(response);
            console.log(response.data);
            that.setState({
                students:students,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                window.location.href="/teachers/log"
            message.error("修改成绩失败");
            that.setState({
                loading:false
            })
            console.log(error);
        })
        
    }
    HandleCancelClick(s_index)
    {
        var students = JSON.parse(JSON.stringify(this.state.students));
        students[s_index].isEdit = false;
        students[s_index].temp.score = students[s_index].score;
        students[s_index].temp.gradeStatus = students[s_index].gradeStatus;
        this.setState({
            students:students
        })
    }
    HandleChangeClick(s_index)
    {
        let students = this.state.students;
        console.log(s_index);
        console.log(this.state.students);
        console.log(this.state.temp);
        students.forEach(function(item, index, arr) {
            if(index == s_index)
                item.isEdit = true
        });
        console.log(students);
        this.setState({
            students:JSON.parse(JSON.stringify(students)) 
        })  
    }
    componentDidMount()
    {
        this.GetOneSchedule();
    }
    GetOneSchedule()
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('/api/teachers/GetOneSchedule',
        {
            params:{'schedule_id':that.state.id},
            headers:{'jwt':localStorage.getItem("teacher_token")}
        })
        .then(function (response) {
            console.log(response);
            console.log(response.data);
            var data = response.data;
            var arr = data.students;
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
                arr[i].isEdit = false;
                arr[i].temp = {score:arr[i].score,gradeStatus:null};
            }
            console.log(arr);
            that.setState({
                isOver:data.isOver,
                year:data.year,
                half:data.half,
                currentNum:data.currentNum,
                maxNum:data.maxNum,
                place:data.place,
                scheduleId:data.scheduleId,
                campus:data.campus,
                lessonName:data.lessonName,
                students: arr,
                canImportGrade:data.canImportGrade,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                window.location.href="/teachers/log";
            message.error("该排课不存在");
            that.setState({
                loading:false
            })
            console.log(error);
        })
    }
    ExportNames()
    {
        var that = this;
        axios.get('/api/teachers/ExportStudents',
        {
            params:{'schedule_id':that.state.id},
            headers:{'jwt':localStorage.getItem("teacher_token")}
        })
        .then(function (response) {
            console.log(response);
            console.log(response.data);
            var data = response.data;
            window.location.href=response.data.detail;
        })
        .catch(function (error) {
            if(isExpire(error))
                window.location.href="/teachers/log"
            console.log(error);
        })
    }
    RegisterGrades(info) 
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
            message.success(` 成功上传成绩`);
            this.setState({"loading":false});
            window.location.reload();
        } 
        else if (info.file.status === 'error') 
        {
            this.setState({"loading":false});
            message.error(`上传成绩失败`);
        }
        
    };
    render(){
        //var that = this;
        return (
            <Spin spinning={this.state.loading}>
            <div>
                <Row> 
                <Col  span={3} offset={0} >
                <b>课程序号:{this.state.scheduleId}</b>
                </Col>
                <Col  span={3} offset={2} >
                <b>课程名:{this.state.lessonName}</b>
                </Col>
                <Col  span={3} offset={2} >
                <b>课程学期:{this.state.year+this.state.half-1}-{this.state.year+this.state.half}学年</b>
                </Col>
                <Col span={3} offset={2}>
                <b>是否完结:{this.state.isOver==1?"是":"否"}</b>
                </Col>
                <Col span={3} offset={2}>
                <b>课程人数/人数上限:{this.state.currentNum+"/"+this.state.maxNum}</b>
                </Col>
                </Row>
                <br/>
                <Col span={3} offset={12}>
                <Spin spinning={this.state.loading} size="large"></Spin>
                </Col>
                
                
                <Row>
                <Col span={3} offset={1}>
                <Button onClick={this.ExportNames.bind(this)}>导出名单</Button>
                </Col>
                <Col span={3} offset={15}>
                    <Upload
                    accept=".xls, .xlsx"
                    name="file"
                    className="avatar-uploader"
                    showUploadList={false}
                    action="/api/teachers/RegisterScoresByFile"
                    data={{"schedule_id":this.state.id}}
                    headers={{
                        'jwt':localStorage.getItem("teacher_token")
                    }}
                    onChange={this.RegisterGrades.bind(this)}
                    >
                        <Button  disabled={this.state.isOver == 1 || this.state.canImportGrade == 0}>
                        <UploadOutlined /> 上传成绩
                        </Button>
                    </Upload>
                </Col>
                <Button>
                <a href="http://139.224.50.124/template/教师上传成绩模板.xlsx">查看模板文件</a>
                </Button>
                </Row>
                <br/>
                <br/>
                
            <Table tableLayout='fixed' dataSource={this.state.students} columns={this.columns1}></Table>
            </div>
            </Spin>
        );
    }

 
}

export default OneSchedule;