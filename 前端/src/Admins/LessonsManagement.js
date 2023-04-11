
import { Button, DatePicker,Table,Spin ,Select,Space, Input,Upload,Row,Col,Modal,Form, message} from 'antd';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import { isExpire } from '../component/HandleAdminsToken';
import 'moment/locale/zh-cn';
import { UploadOutlined } from '@ant-design/icons';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
import locale from 'antd/lib/date-picker/locale/zh_CN';
import moment from "moment";

const {Search} = Input;
const {Option} = Select;

const number = ['','一','二','三','四','五','六','七','八','九','十'];
class LessonsManagement extends React.Component
{
    constructor(props)
    {
        super(props);
        this.myref=React.createRef();
        this.state = ({
            page:1,
            pageSize:20,
            lessons:[],
            total:0,
            visible:false,
            refs:[],
            loading:false
        });
        this.columns1=
        [
            {
                title: '课程号',
                key: 'lessonId',
                render :(text,record)=>(
                    <p>{record.lesson.lessonId}</p>
                )
            },
            {
                title: '课程名称',
                key: 'lessonName',
                render :(text,record,index)=>{
                    if(record.isEdit)
                    {
                        return(
                            <Input onChange={this.handleNameChange.bind(this,index)}
                            value={record.temp.lessonName}
                            style={{ width: 80 }}/>
                        )
                    }
                    else
                    {
                        return <p>{record.lesson.lessonName}</p>
                    }  
                }
            },
            {
                title: '课程类型',
                key: 'type',
                render :(text,record,index)=>{
                    if(record.isEdit)
                    {
                        return(
                            <Select 
                            onChange={this.handletypeChange.bind(this,index)}
                            value={record.temp.type}
                                style={{ width: 100 }}>
                                <Option value="专业课">专业课</Option>
                                <Option value="通识选修">通识选修</Option>
                                <Option value="通识必修">通识必修</Option>
                                <Option value="体育课">体育课</Option>
                            </Select>
                        )
                    }
                    else
                    {
                        return <p>{record.lesson.type}</p>
                    }  
                }
            },
            {
                title: '学分',
                key: 'credit',
                render :(text,record)=>(
                    <p>{record.lesson.credit}</p>
                )
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
                            style={{ width: 80 }}/>
                        )
                    }
                    else
                    {
                        return <p>{record.lesson.note}</p>
                    }  
                }
            },
            {
                title: '能选课的专业',
                key: 'needDepart',
                render :(text,record,index)=>{
                    if(record.isEdit)
                    {
                        return(
                            <Select onChange={this.handleneedDepartChange.bind(this,index)} 
                                value={record.temp.needDepart}
                                style={{ width: 100 }}>
                                <Option value="软件工程">软件工程</Option>
                                <Option value="数学系">数学系</Option>
                                <Option value="物理系">物理系</Option>
                                <Option value="艺术与传媒">艺术与传媒</Option>
                                <Option value="all">所有</Option>
                            </Select>
                        )
                    }
                    else
                    {
                        return <p>{record.lesson.needDepart=="all"?"所有":record.lesson.needDepart}</p>
                    }  
                }
            },
            {
                title: '选课要求',
                key: 'requirements',
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
                        //this.refs[index] = ref;
                        return(
                        <Form ref={ref}>
                        <Form.List name="requires" initialValue={record.requirements}>
                            {(fields, { add, remove }) => (
                            <>
                                {fields.map(({ key, name, ...restField }) => (
                                <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                                    <Form.Item
                                    {...restField}
                                    name={[name, 'inYear']}
                                    rules={[{ required: true, message: '缺少学生年级' }]}
                                    >
                                    <Input type='number' placeholder='请输入年级'></Input>
                                    </Form.Item>
                                    <Form.Item
                                    {...restField}
                                    name={[name, 'minGrade']}
                                    rules={[{ required: true, message: '缺少最低入学年份' }]}
                                    >
                                        <Select placeholder="最低入学年份"  
                                        
                                        style={{ width: 90 }}>
                                        <Option value={1}>1</Option>
                                        <Option value={2}>2</Option>
                                        <Option value={3}>3</Option>
                                        <Option value={4}>4</Option>
                                        <Option value={5}>5</Option>
                                        <Option value={6}>6</Option>
                                        </Select>
                                    </Form.Item>
                                    <Form.Item
                                    {...restField}
                                    name={[name, 'maxGrade']}
                                    rules={[{ required: true, message: '缺少最高入学年份' }]}
                                    >
                                        <Select placeholder="最高入学年份"  
                                        
                                        style={{ width: 90 }}>
                                        <Option value={1}>1</Option>
                                        <Option value={2}>2</Option>
                                        <Option value={3}>3</Option>
                                        <Option value={4}>4</Option>
                                        <Option value={5}>5</Option>
                                        <Option value={6}>6</Option>
                                        </Select>
                                    </Form.Item>
                                    <MinusCircleOutlined onClick={() => remove(name)} />
                                </Space>
                                ))}
                                <Form.Item>
                                <Button type="dashed" onClick={() => add()} block icon={<PlusOutlined />}>
                                    增加选课条件
                                </Button>
                                </Form.Item>
                            </>
                            )}
                        </Form.List>
                        </Form>)
                    }
                    else
                    {
                        return(record.requirements.map(function(item,index,array){
                            return (<p>{item.inYear+"年级 大"+number[item.minGrade]+"-大"+number[item.maxGrade]}</p>)
                        }))
                        
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
                                <Button  type='primary' onClick={this.HandleEditClick.bind(this,index)}>编辑 
                                </Button>
                            </Space>
                        )
                    }
                    
                } 
            }
        ];
    }
    handleNameChange(s_index,event)
    {
        var lessons = JSON.parse(JSON.stringify(this.state.lessons));
        lessons[s_index].temp.lessonName = event.target.value;
        this.setState({lessons: lessons});
    }
    handletypeChange(s_index,value)
    {
        var lessons = JSON.parse(JSON.stringify(this.state.lessons));
        lessons[s_index].temp.type = value;
        this.setState({lessons: lessons});
    }
    handlenoteChange(s_index,event)
    {
        var lessons = JSON.parse(JSON.stringify(this.state.lessons));
        lessons[s_index].temp.note = event.target.value;
        this.setState({lessons: lessons});
    }
    handleneedDepartChange(s_index,value)
    {
        var lessons = JSON.parse(JSON.stringify(this.state.lessons));
        lessons[s_index].temp.needDepart = value;
        this.setState({lessons: lessons});
    }
    HandleYesClick(s_index)
    {
        var _lessons = JSON.parse(JSON.stringify(this.state.lessons));
        var that = this;
        var requires = this.state.refs[s_index].current.getFieldsValue().requires;
        var lessons = _lessons.map(function(item,index,array){
            if(index == s_index)
            {
                item.isEdit = false;
                item.lesson.lessonName = item.temp.lessonName ;
                item.lesson.needDepart = item.temp.needDepart ;
                item.lesson.type = item.temp.type ;
                item.requirements = requires;
                item.lesson.note = item.temp.note ;
            }  
            return item;
        })
        console.log(this.state.refs[s_index].current.getFieldsValue().requires);
        
        console.log(requires);
        if(lessons[s_index].temp.lessonName == null  || lessons[s_index].temp.type ==null
            || requires == null ||lessons[s_index].temp.needDepart == null) 
        {
            message.error("除备注外不能为空");
            return;
        }
        console.log(lessons[s_index].temp);
        console.log(lessons[s_index].temp.note);
        
        axios.post('/api/admins/ModifyLesson',
        {
            "lesson_id":lessons[s_index].lesson.lessonId,
            "lesson_name":lessons[s_index].temp.lessonName,
            "type":lessons[s_index].temp.type,
            "requires":requires,
            "need_depart":lessons[s_index].temp.needDepart,
            "note":lessons[s_index].temp.note,
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            message.info("成功修改课程信息");
            console.log(response);
            console.log(response.data);
            that.setState({
                lessons:lessons
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
        var lessons = JSON.parse(JSON.stringify(this.state.lessons));
        console.log(this.state.refs);
        console.log(this.state.refs[s_index]);
        console.log(this.state.refs[s_index].current.getFieldsValue());
        this.state.refs[s_index].current.setFieldsValue({
            requires:lessons[s_index].requirements
        });
        lessons[s_index].isEdit = false;
        lessons[s_index].temp.lessonName = lessons[s_index].lesson.lessonName;
        lessons[s_index].temp.type = lessons[s_index].lesson.type;
        //lessons[s_index].temp.requirements = lessons[s_index].requirements;
        lessons[s_index].temp.note = lessons[s_index].lesson.note;
        lessons[s_index].temp.needDepart = lessons[s_index].lesson.needDepart;
        this.setState({
            lessons:lessons
        })
    }
    HandleEditClick(s_index)
    {
        let lessons = this.state.lessons;
        console.log(s_index);
        console.log(this.state.lessons);
        lessons.forEach(function(item, index, arr) {
            if(index == s_index)
                item.isEdit = true;
        });
        console.log(lessons);
        this.setState({
            lessons:JSON.parse(JSON.stringify(lessons)) 
        })  
    }
    componentDidMount()
    {
        this.Get_lessons(this.state.page,this.state.pageSize);
    }
    Get_lessons( page, pageSize)
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('/api/admins/GetLessons',
        {
            params:{"page":page,
                    "page_size":pageSize},
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            var arr = response.data.lessons;
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
                arr[i].isEdit = false;  
                arr[i].temp = { lessonName:arr[i].lesson.lessonName,
                                type:arr[i].lesson.type,
                                requirements:arr[i].requirements,
                                note:arr[i].lesson.note,
                                needDepart:arr[i].lesson.needDepart};
            }
            console.log(arr);
            that.setState({
                lessons:arr,
                total:response.data.total,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                lessons:[],
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
        this.Get_lessons(page,pageSize);
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
            this.Get_lessons(this.state.page,this.state.pageSize);
            return;
        }
        this.setState({
            page:1
        })
        var that = this;
        axios.get('/api/admins/QueryLesson',
        {
            params:{"lesson_id":value},
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
                lessons:new_arr,
                total:1
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                lessons:[],
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
        const {lesson_name,type,credit,preq_id,note,need_depart,identity,requires} = this.myref.current.getFieldsValue();
        console.log(requires);
        var _requires = requires.map(function(value,index){
            value.inYear = value.Inyear._d.getFullYear();
            value.Inyear = undefined;
            return value;
        })
        console.log(requires);
        if(lesson_name == null||
            type == null||
            credit == null||
            need_depart == null||
            identity == null ||
            _requires == null
            )
        {
            return;
        }
        var that = this;
        axios.post('/api/admins/AddLesson',
        {
            lesson_name:lesson_name,
            type:type,
            credit:credit,
            preq_id:preq_id==null?null:preq_id,
            note:note==null?null:note,
            need_depart:need_depart,
            identity:identity,
            requires:_requires
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            message.info("成功添加课程");
            that.setState({
                visible:false
            })
            that.Get_lessons(that.state.page,that.state.pageSize);
            that.myref.current.resetFields();
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            message.error("添加课程失败");
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
            message.success(` 成功录入课程信息`);
            that.Get_lessons(that.state.page,that.state.pageSize);
        } 
        else if (info.file.status === 'error') 
        {
            this.setState({"loading":false});
            message.error(`录入课程信息失败`);
        }
        
    };
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

                <Form.Item  label="课程名" required>
                    <Form.Item 
                    style={{ display: 'inline-flex'}}
                    name="lesson_name" 
                    rules={[{ required: true, message: '课程名为空' }]}
                    >
                    <Input placeholder="请入课程名"/>
                    </Form.Item>
                    <Form.Item 
                    style={{ display: 'inline-flex',width: 'calc(55% - 4px)', marginLeft: '8px' }} 
                    name="type" 
                    label="课程类型" 
                    rules={[{ required: true, message: '课程类型为空' }]}
                    >
                    <Select 
                        style={{ width: 100 }}>
                        <Option value="专业课">专业课</Option>
                        <Option value="通识选修">通识选修</Option>
                        <Option value="通识必修">通识必修</Option>
                        <Option value="体育课">体育课</Option>
                    </Select>
                    </Form.Item>
                </Form.Item>

                <Form.Item  label="学分" required>
                    <Form.Item 
                    style={{ display: 'inline-flex'}}
                    name="credit" 
                    rules={[{ required: true, message: '课程名为空' }]}
                    >
                    <Input placeholder="请入学分数" type="number"/>
                    </Form.Item>
                    <Form.Item 
                    style={{ display: 'inline-flex', marginLeft: '8px' }} 
                    name="preq_id" 
                    label="以往课程号" 
                    >
                    <Input placeholder="(可选)" type="number"/>
                    </Form.Item>
                </Form.Item>

                <Form.Item  label="能选课的专业" required>
                    <Form.Item 
                    style={{ display: 'inline-flex', marginLeft: '8px' }} 
                    name="need_depart" 
                    rules={[{ required: true, message: '能选课的专业为空' }]}
                    >
                    <Select 
                        style={{ width: 100 }}>
                        <Option value="软件工程">软件工程</Option>
                        <Option value="数学系">数学系</Option>
                        <Option value="物理系">物理系</Option>
                        <Option value="艺术与传媒">艺术与传媒</Option>
                        <Option value="all">所有</Option>
                    </Select>
                    </Form.Item>
                    <Form.Item 
                    style={{ display: 'inline-flex', marginLeft: '8px' }}
                    label="备注"
                    name="note" 
                    >
                    <Input placeholder="(可选)"/>
                    </Form.Item>
                    
                </Form.Item>

                <Form.Item 
                label="哪类学生的课"
                style={{ display: 'inline-flex'}}
                name="identity" 
                rules={[{ required: true, message: '课程名为空' }]}
                >
                <Select 
                    style={{ width: 100 }}>
                    <Option value="本科生">本科生</Option>
                    <Option value="研究生">研究生</Option>
                    <Option value="博士生">博士生</Option>
                </Select>
                </Form.Item>
                <br></br>
                <label>学生选课年级要求:</label>
                <br></br>
                <br></br>
                <Form.List name="requires" >
                    {(fields, { add, remove }) => (
                    <>
                        {fields.map(({ key, name, ...restField }) => (
                        <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                            <Form.Item
                            {...restField}
                            name={[name, 'Inyear']}
                            rules={[{ required: true, message: '缺少学生年级' }]}
                            >
                            <DatePicker placeholder='学生年级' picker="year" locale={locale}></DatePicker>
                            </Form.Item>
                            <Form.Item
                            {...restField}
                            name={[name, 'MinGrade']}
                            rules={[{ required: true, message: '缺少最低入学年份' }]}
                            >
                                <Select placeholder="最高入学年份"  
                                
                                style={{ width: 90 }}>
                                <Option value={1}>1</Option>
                                <Option value={2}>2</Option>
                                <Option value={3}>3</Option>
                                <Option value={4}>4</Option>
                                <Option value={5}>5</Option>
                                <Option value={6}>6</Option>
                                </Select>
                            </Form.Item>
                            <Form.Item
                            {...restField}
                            name={[name, 'MaxGrade']}
                            rules={[{ required: true, message: '缺少最高入学年份' }]}
                            >
                                <Select placeholder="最高入学年份"  
                                
                                style={{ width: 90 }}>
                                <Option value={1}>1</Option>
                                <Option value={2}>2</Option>
                                <Option value={3}>3</Option>
                                <Option value={4}>4</Option>
                                <Option value={5}>5</Option>
                                <Option value={6}>6</Option>
                                </Select>
                            </Form.Item>
                            <MinusCircleOutlined onClick={() => remove(name)} />
                        </Space>
                        ))}
                        <Form.Item>
                        <Button type="dashed" onClick={() => add()} block icon={<PlusOutlined />}>
                            增加选课条件
                        </Button>
                        </Form.Item>
                    </>
                    )}
                </Form.List>
                
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
                <Search type="number" allowClear={true} placeholder="输入课程号" onSearch={this.onSearch.bind(this)} style={{ width: 200 }} />
                &nbsp;&nbsp;&nbsp;
                <Button onClick={this.OpenaddNew.bind(this)}>添加课程</Button>
                &nbsp;&nbsp;&nbsp;
                    <Upload
                    accept=".xls, .xlsx"
                    name="file"
                    className="avatar-uploader"
                    showUploadList={false}
                    action="/api/admins/AddLessonsByFile"
                    headers={{
                        'jwt':localStorage.getItem("admin_token")
                    }}
                    onChange={this.HandleUploadChange.bind(this)}
                    >
                        <Button  >
                        <UploadOutlined /> 上传文件添加课程
                        </Button>
                    </Upload>
                    &nbsp;&nbsp;&nbsp;
                    <Button>
                    <a href="http://139.224.50.124/template/课程信息录入模板.xlsx">查看模板文件</a>
                    </Button>
                </Row>
                <br/><br/>
                <br/>
                <Table dataSource={this.state.lessons} 
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

export default LessonsManagement;