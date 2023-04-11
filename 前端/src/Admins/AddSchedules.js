
import { Button, message,Radio,Row,Upload,Table ,Select,Space, Input,Modal, Form, Spin} from 'antd';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
import { UploadOutlined } from '@ant-design/icons';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import { isExpire } from '../component/HandleAdminsToken';
import 'moment/locale/zh-cn';
import locale from 'antd/lib/date-picker/locale/zh_CN';
const {Search} = Input;
const {Option} = Select;
const number = ['','一','二','三','四','五','六','七','八','九','十'];
class AddSchedules extends React.Component
{
    constructor(props)
    {
        super(props);
        this.state = ({
            year:null,
            half:null,
            page:1,
            pageSize:20,
            lessons:[],
            visible:false,
            lesson_id:null,
            loading:false
        });
        this.myref = React.createRef();
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
                        return <p>{record.lesson.lessonName}</p>
                }
            },
            {
                title: '课程类型',
                key: 'type',
                render :(text,record,index)=>{
                        return <p>{record.lesson.type}</p>
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
                        return <p>{record.lesson.note}</p>
                }
            },
            {
                title: '能选课的专业',
                key: 'needDepart',
                render :(text,record,index)=>{
                    return <p>{record.lesson.needDepart=="all"?"所有":record.lesson.needDepart}</p>
                }
            },
            {
                title: '选课要求',
                key: 'requirements',
                render :(text,record,index)=>{
                        return(record.requirements.map(function(item,index,array){
                            return (<p>{item.inYear+"年级 大"+number[item.minGrade]+"-大"+number[item.maxGrade]}</p>)
                        }))
                     }
            },
            {
                title: '操作',
                key: 'action',
                render: (text, record,index) =>{
                    return(     
                        <Space size="middle">
                            <Button  type='primary' onClick={this.HandleOpenClick.bind(this,record)}>增加排课 
                            </Button>
                        </Space>
                    )
                } 
            }
        ];
//<Link to={{pathname:'/teachers/schedule',search:'?id='+str,state:{"p":str}}}></Link>
    }
    HandleOpenClick(item)
    {
        this.setState({
            visible:true,
            lesson_id:item.lesson.lessonId
        })
    }
    HandleAddClick()
    {
        console.log(this.state.lesson_id);
        const {teacher_id,year,half,begin_week,end_week,place,max_num,note,teaching_material,can_retake,campus,times} = 
        this.myref.current.getFieldsValue();
        if(teacher_id == null||year==null||half==null||begin_week==null||end_week==null||place==null
            || max_num==null||can_retake==null||campus==null)
        {
            return;
        }
        var that = this;
        axios.post('/api/admins/AddSchedule',
        {
            lesson_id:that.state.lesson_id,
            year:year,
            half:half,
            teacher_id:teacher_id,
            begin_week:begin_week,
            end_week:end_week,
            max_num:max_num,
            place:place,
            teaching_material:teaching_material==null?null:teaching_material,
            note:note==null?null:note,
            can_retake:can_retake,
            campus:campus,
            times:times
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            message.info("成功增加排课");
            that.setState({
                visible:false
            })
            that.myref.current.resetFields();
            that.Get_lessons(that.state.page,that.state.pageSize);
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            message.error("增加排课失败");
            const arr = Object.keys(error);
            console.log(arr);
            console.log(error.response.data);
        });
    }
    componentDidMount()
    {
        this.Get_lessons(this.state.page,this.state.pageSize);
    }
    HandleClick(item)
    {
        window.open('/teachers/schedule?id='+item.scheduleId/*,"_blank"*/);
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
            message.success(` 成功录入排课信息`);
            that.Get_lessons(that.state.page,that.state.pageSize);
        } 
        else if (info.file.status === 'error') 
        {
            this.setState({"loading":false});
            message.error(`录入排课信息失败`);
        }
        
    };
    showTotal()
    {
        return `共 ${this.state.total} 项记录`
    }
    cancelModal()
    {
        this.setState({
            visible:false
        })
        this.myref.current.resetFields();
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
    render(){
        //var that = this;
        return (
            <Spin spinning={this.state.loading}>
            <div>
                <Modal  width={800}
            visible={this.state.visible} 
            okText="关闭"
            cancelText="取消"
            onOk={this.cancelModal.bind(this)} 
            onCancel={this.cancelModal.bind(this)}
            bodyStyle={{height: '500px', overflowY: 'auto',overflowX:'auto'}}>
            <label>课程号：{this.state.lesson_id} </label>
            <Form ref={this.myref}>

                <Form.Item  label="年份" required>
                    <Form.Item 
                    style={{ display: 'inline-flex'}}
                    name="year" 
                    rules={[{ required: true, message: '年份为空' }]}
                    >
                    <Input type="number" placeholder="请输入年份"/>
                    </Form.Item>
                    <Form.Item 
                    style={{ display: 'inline-flex',width: 'calc(55% - 4px)', marginLeft: '8px' }} 
                    name="half" 
                    label="上下半年" 
                    rules={[{ required: true, message: '半年为空' }]}
                    >
                    <Select 
                        style={{ width: 100 }}>
                        <Option value="0">上半年</Option>
                        <Option value="1">下半年</Option>
                    </Select>
                    </Form.Item>
                </Form.Item>

                <Form.Item label="开始周" required>
                    <Form.Item 
                    style={{ display: 'inline-flex'}}
                    name="begin_week" 
                    rules={[{ required: true, message: '课程名为空' }]}
                    >
                    <Input type="number" placeholder="请入开始周"/>
                    </Form.Item>
                    <Form.Item 
                    style={{ display: 'inline-flex',width: 'calc(55% - 4px)', marginLeft: '8px' }} 
                    name="end_week" 
                    label="结束周" 
                    rules={[{ required: true, message: '课程类型为空' }]}
                    >
                    <Input type="number" placeholder="请入结束周"/>
                    </Form.Item>
                </Form.Item>

                <Form.Item 
                    style={{ display: 'inline-flex',width: 'calc(55% - 4px)', marginLeft: '8px' }} 
                    name="teacher_id" 
                    label="教师工号" 
                    rules={[{ required: true, message: '课程类型为空' }]}
                    >
                    <Input type="number" placeholder="请入教师工号"/>
                </Form.Item>

                <Form.Item label="校区" required>
                    <Form.Item 
                    style={{ display: 'inline-flex'}}
                    name="campus" 
                    rules={[{ required: true, message: '校区为空' }]}
                    >
                    <Select 
                        style={{ width: 100 }}>
                        <Option value="四平路">四平路</Option>
                        <Option value="嘉定">嘉定</Option>
                    </Select>   
                    </Form.Item>
                    <Form.Item 
                    style={{ display: 'inline-flex',width: 'calc(55% - 4px)', marginLeft: '8px' }} 
                    name="place" 
                    label="上课地点" 
                    rules={[{ required: true, message: '上课地点为空' }]}
                    >
                    <Input  placeholder="上课地点"/>
                    </Form.Item>
                </Form.Item>

                <Form.Item label="人数上限" required>
                    <Form.Item 
                    style={{ display: 'inline-flex'}}
                    name="max_num" 
                    rules={[{ required: true, message: '人数上限为空' }]}
                    >
                        <Input type="number" placeholder="请入结束周"/>
                     
                    </Form.Item>
                    <Form.Item 
                    style={{ display: 'inline-flex',width: 'calc(55% - 4px)', marginLeft: '8px' }} 
                    name="can_retake" 
                    label="是否允许重修" 
                    rules={[{ required: true, message: '是否允许重修为空' }]}
                    >
                    <Radio.Group >
                        <Radio value={1}>是</Radio>
                        <Radio value={0}>否</Radio>
                    </Radio.Group>
                    </Form.Item>
                </Form.Item>

                <Form.Item label="备注" >
                    <Form.Item 
                    style={{ display: 'inline-flex'}}
                    name="note" 
                    >
                    <Input  placeholder="请入备注(可选)"/>
                     
                    </Form.Item>
                    <Form.Item 
                    style={{ display: 'inline-flex',width: 'calc(55% - 4px)', marginLeft: '8px' }} 
                    name="teaching_material" 
                    label="教材" 
                    >
                    <Input  placeholder="请入教材(可选)"/>
                    </Form.Item>
                </Form.Item>

                <label>上课时间:</label>
                <br></br>
                <br></br>
                <Form.List name="times" /* initialValue={record.times} */>
                    {(fields, { add, remove }) => (
                    <>
                        {fields.map(({ key, name, ...restField }) => (
                        <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                            <Form.Item
                            {...restField}
                            name={[name, 'DayWeek']}
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
                                <Option value="1">1</Option>
                                <Option value="2">2</Option>
                                <Option value="3">3</Option>
                                <Option value="4">4</Option>
                                <Option value="5">5</Option>
                                <Option value="6">6</Option>
                                <Option value="7">7</Option>
                                <Option value="8">8</Option>
                                <Option value="9">9</Option>
                                <Option value="10">10</Option>
                                <Option value="11">11</Option>
                                <Option value="12">12</Option>
                            </Select>
                            </Form.Item>
                            <Form.Item
                            {...restField}
                            name={[name, 'endSection']}
                            rules={[{ required: true, message: '缺少结束的节数' }]}
                            >
                            <Select placeholder="第几节结束"  
                                style={{ width: 90 }}>
                                <Option value="1">1</Option>
                                <Option value="2">2</Option>
                                <Option value="3">3</Option>
                                <Option value="4">4</Option>
                                <Option value="5">5</Option>
                                <Option value="6">6</Option>
                                <Option value="7">7</Option>
                                <Option value="8">8</Option>
                                <Option value="9">9</Option>
                                <Option value="10">10</Option>
                                <Option value="11">11</Option>
                                <Option value="12">12</Option>
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

                <Form.Item
                wrapperCol={{
                    offset: 11,
                    span: 12,
                }}
                >
                <Button type="primary" htmlType="submit" onClick={this.HandleAddClick.bind(this)}>
                    确定
                </Button>
                </Form.Item> 

            </Form>
            </Modal>
                <Row>
                <Search type="number" allowClear={true} placeholder="输入课程号" onSearch={this.onSearch.bind(this)} style={{ width: 200 }} />
                &nbsp;&nbsp;&nbsp;
                    <Upload
                    accept=".xls, .xlsx"
                    name="file"
                    className="avatar-uploader"
                    showUploadList={false}
                    action="/api/admins/AddSchedulesByFile"
                    headers={{
                        'jwt':localStorage.getItem("admin_token")
                    }}
                    onChange={this.HandleUploadChange.bind(this)}
                    >
                        <Button  >
                        <UploadOutlined /> 上传文件添加排课
                        </Button>
                    </Upload>
                    &nbsp;&nbsp;&nbsp;
                    <Button>
                    <a href="http://139.224.50.124/template/排课录入模板.xlsx">查看模板文件</a>
                    </Button>
                </Row>
                <br></br>
                <br></br>
                <Table dataSource={this.state.lessons} 
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

export default AddSchedules;