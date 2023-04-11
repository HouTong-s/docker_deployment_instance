import { Button,Table ,Space, Modal,Pagination, Spin} from 'antd';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import { isExpire } from '../component/HandleTokenExpire';
import { Typography,message } from 'antd';

const { Paragraph, Text } = Typography;
class GetNotice extends React.Component
{
    constructor(props)
    {
        super(props);
        this.state = ({
            has_notice:true,
            visible:false,
            notices:[],
            total:0,
            content:'',
            loading:false
        });
        this.columns1=
        [
            {
                title: '标题',
                key: 'title',
                dataIndex:'title'
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
                title: '操作',
                key: 'action',
                render: (text, record) => (
                <Space size="middle">
                    <Button type='primary' onClick={this.handleClick.bind(this,record)}>查看详细信息 </Button>
                </Space>
                ),
            },
        ];
    }
    handleClick(item)
    {
        console.log(item.content);
        this.setState({
            visible : true,
            content : item.content
        });
    }
    componentDidMount()
    {
        this.GetNotices();
    }
    GetNotices()
    {
        this.QueryPages(1,10);
    }
    QueryPages(page , pageSize)
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('/api/students/GetNotices',
        {
            params:{page:page,page_size:pageSize},
            headers:{'jwt':localStorage.getItem("token")}
        })
        .then(function (response) {
            console.log(response);
            console.log(response.data);
            
            that.setState({
                total:response.data.total,
                notices:response.data.notices,
                loading:false
            })
        })
        .catch(function (error) {
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                loading:false
            })
            if(error.response.status == 404)
            {
                that.setState({
                    has_notice:false
                })
            }
            else if(error.response.status == 400)
            {
                message.error("请求页码错误");
            }
            console.log(error);
        })
    }
    cancelModal()
    {
        this.setState({
            visible:false
        })
    }
    handlePagination(page, pageSize)
    {
        this.QueryPages(page,pageSize);
    }
    showTotal(total)
    {
        console.log(total);
        return `共 ${total} 项记录`
    }
    render(){
        var that = this;
        if(!this.state.has_notice)
        {
            return(<p>还尚未有通知 </p>)
        }
        return (
            <Spin spinning={this.state.loading}>
            <div>
                <Table dataSource={this.state.notices} 
                columns={this.columns1} 
                pagination={{
                    showSizeChanger:true,
                    onChange:this.handlePagination.bind(this),
                    defaultPageSize:20,
                    defaultCurrent:1,
                    total:this.state.total,
                    showTotal:that.showTotal.bind(this)
                }}
                scroll={{ x: "90%" }}>
                </Table>
                <Modal  width={800}
            visible={this.state.visible} 
            onOk={this.cancelModal.bind(this)} 
            onCancel={this.cancelModal.bind(this)}
            bodyStyle={{height: '300px', overflowY: 'auto',overflowX:'auto'}}>
            <Paragraph >{this.state.content} </Paragraph>
            </Modal>
            </div>
            </Spin>
            );
    }

 
}

export default GetNotice;