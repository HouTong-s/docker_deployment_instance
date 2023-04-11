
import { Button, DatePicker,Table ,Select,Space, Spin,Input,Upload,Row,Col,Modal,Form, message, Popconfirm} from 'antd';
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
class NoticesManagement extends React.Component
{
    constructor(props)
    {
        super(props);
        this.myref=React.createRef();
        this.changeref = React.createRef();
        this.state = ({
            page:1,
            pageSize:20,
            notices:[],
            total:0,
            visible1:false,
            loading:false
        });
        this.columns1=
        [
            {
                title: '通知序号',
                key: 'notice_id',
                dataIndex:'notice_id'
            },
            {
                title: '标题',
                key: 'title',
                render:(text,record,index)=>{
                    if(record.isEdit)
                    {
                        return(
                        <Input
                         value={record.temp.title} 
                         placeholder='请输入内容'
                          style={{"width":"100px"}}
                          onChange={this.handleTitleChange.bind(this,index)}> 
                        </Input>
                        ) 
                    }
                    else
                    {
                        return <>{record.title} </>
                    }
                }
            },
            {
                title: '发布时间',
                key: 'time',
                render: (text, record) => {
                    var times =  record.time.split("T");
                    return (<p>{times[0]}  {times[1]}</p>)
                },
            },
            {
                title: '内容',
                key: 'content',
                render:(text,record,index)=>{
                    if(record.isEdit)
                    {
                        return(
                        <Input.TextArea
                         value={record.temp.content} 
                         placeholder='请输入内容'
                          style={{"height":"200px"}}
                          onChange={this.handleContentChange.bind(this,index)}> 
                        </Input.TextArea>
                        ) 
                    }
                    else
                    {
                        return <>{record.content} </>
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
                                <Button onClick={this.HandleYesClick.bind(this,index)} type='primary'> 确定 
                                </Button>
                                <Button onClick={this.HandleCancelClick.bind(this,index)} type='primary'> 取消 
                                </Button>
                            </Space>
                        )
                    }
                    else
                    {
                        return(     
                            <Space size="middle">
                                <Button onClick={this.HandleChangeClick.bind(this,index)} type='primary'> 修改 
                                </Button>
                            </Space>
                        )
                    }
                    
                } 
            }
        ];
    }
    handleTitleChange(index,event)
    {
        var notices = this.state.notices;
        notices[index].temp.title = event.target.value;
        this.setState({
            notices:JSON.parse(JSON.stringify(notices))
        })
    }
    handleContentChange(index,event)
    {
        var notices = this.state.notices;
        notices[index].temp.content = event.target.value;
        this.setState({
            notices:JSON.parse(JSON.stringify(notices))
        })
    }
    HandleChangeClick(index)
    {
        var notices = this.state.notices;
        notices[index].isEdit = true;
        this.setState({
            notices:JSON.parse(JSON.stringify(notices))
        })
    }
    HandleYesClick(s_index)
    {
        var notices = JSON.parse(JSON.stringify(this.state.notices));
        var that = this;
        notices = notices.map(function(item,index,array){
            if(index == s_index)
            {
                item.isEdit = false;
                item.title = item.temp.title ;
                item.content = item.temp.content ;
            }  
            return item;
        })
        if(notices[s_index].temp.title == null  || notices[s_index].temp.content ==null) 
        {
            return;
        }
        axios.post('/api/admins/ModifyNotice',
        {
            notice_id:notices[s_index].notice_id,
            title:notices[s_index].temp.title,
            content:notices[s_index].temp.content  
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            message.info("成功修改通知");
            that.setState({
                visible1:false
            })
            that.Get_notices(that.state.page,that.state.pageSize);
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            message.error("修改通知失败");
            const arr = Object.keys(error);
            console.log(arr);
            console.log(error.response.data);
        });
        
    }
    HandleCancelClick(s_index)
    {
        var notices = JSON.parse(JSON.stringify(this.state.notices));
        notices[s_index].isEdit = false;
        notices[s_index].temp.title = notices[s_index].title;
        notices[s_index].temp.content = notices[s_index].content;
        this.setState({
            notices:notices
        })
    }
    componentDidMount()
    {
        this.Get_notices(this.state.page,this.state.pageSize);
    }
    Get_notices( page, pageSize)
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('/api/admins/GetNotices',
        {
            params:{"page":page,
                    "page_size":pageSize},
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            var arr = response.data.notices;
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
                arr[i].isEdit = false;
                arr[i].temp={
                    title:arr[i].title,
                    content:arr[i].content,
                }
            }
            console.log(arr);
            that.setState({
                notices:arr,
                total:response.data.total,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                notices:[],
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
        this.Get_notices(page,pageSize);
    }
    showTotal()
    {
        //console.log(this.state.total);
        return `共 ${this.state.total} 项记录`
    }
    OpenaddNew()
    {
        this.setState({
            visible1:true
        })
    }
    cancelModal()
    {
        this.setState({
            visible1:false
        })
        this.myref.current.resetFields();
    }
    HandleSubmit()
    {
        console.log(this.myref.current.getFieldsValue());
        const {title,content} = this.myref.current.getFieldsValue();
        var that = this;
        if(title == null||content == null)
        {
            return;
        }
        axios.post('/api/admins/AddNotice',
        {
            title:title,
            content:content  
        },
        {
            headers:{'jwt':localStorage.getItem("admin_token")}
        })
        .then(function (response) {
            console.log(response);
            message.info("成功添加通知");
            that.setState({
                visible1:false
            })
            that.myref.current.resetFields();
            that.Get_notices(that.state.page,that.state.pageSize);
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            message.error("添加通知失败");
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
                okText="关闭"
                cancelText="取消"
                visible={this.state.visible1} 
                onOk={this.cancelModal.bind(this)} 
                onCancel={this.cancelModal.bind(this)}
                bodyStyle={{height: '500px', overflowY: 'auto',overflowX:'auto'}}>
                <Form ref={this.myref}>

                    <Form.Item 
                    label="标题"
                    style={{ width: 'calc(55% - 4px)'}} 
                    name="title" 
                    rules={[{ required: true, message: '标题为空' }]}
                    >
                    <Input  placeholder="请输入标题"/>
                    </Form.Item>
                    <Form.Item 
                    style={{ width: 'calc(55% - 4px)'}} 
                    name="content" 
                    label="内容" 
                    rules={[{ required: true, message: '内容为空' }]}
                    >
                    <Input.TextArea placeholder='请输入内容' style={{"height":"200px"}}> </Input.TextArea>
                    </Form.Item>
                <br></br>
                
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
                <Button onClick={this.OpenaddNew.bind(this)}>添加通知</Button>
                &nbsp;&nbsp;&nbsp;
                </Row>
                <br/><br/>
                <br/>
                <Table dataSource={this.state.notices} 
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

export default NoticesManagement;