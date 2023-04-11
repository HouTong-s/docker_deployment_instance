
import { Table , Row, Col, Spin} from 'antd';
import React from 'react';
import 'antd/dist/antd.css';
import axios from 'axios';
import { isExpire } from '../component/HandleTokenExpire';
class GetGrades extends React.Component
{
    constructor(props)
    {
        super(props);
        this.state = ({
            has_grade:true,
            times:[],
            tolog:false,
            loading:false
        });
        this.columns1=
        [
            {
                title: '课程号',
                key: 'lessonId',
                render: (text, record) => (
                    <p> {record.lesson.lessonId}</p>
                 )
            },
            {
                title: '课程名称',
                key: 'lessonName',
                render: (text, record) => (
                    <p> {record.lesson.lessonName}</p>
                 )
            },
            {
                title: '课程类型',
                key: 'type',
                render: (text, record) => (
                    <p> {record.lesson.type}</p>
                 )
            },
            {
                title: '分数',
                dataIndex: 'score',
                key: 'score',
            },
            {
                title: '绩点',
                dataIndex: 'gradePoint',
                key: 'gradePoint',
            },
            {
                title: '是否通过',
                key: 'ispass',
                render: (text, record) => (
                   <p> {record.is_pass?"是":"否"}</p>
                )
            },
            {
                title: '学分',
                key: 'credit',
                render: (text, record) => (
                    <p> {record.lesson.credit}</p>
                 )
            },
            {
                title: '更新时间',
                key: 'time',
                render: (text, record) => {
                    return (<p> {record.time.split("T")[0]+" "+record.time.split("T")[1]}</p>
                 )}
            },
        ];
    }
    componentDidMount()
    {
        this.GetGrades();
    }
    GetGrades()
    {
        var that = this;
        this.setState({
            loading:true
        })
        axios.get('/api/students/GetGrades',
        {
            headers:{'jwt':localStorage.getItem("token")}
        })
        .then(function (response) {
            console.log(response);
            console.log(response.data);
            var arr = response.data.grades;
            var min_year = 10000;
            var max_year = 0;
            var min_half = 1;
            var max_half = 0;
            console.log("1");
            for(var i=0;i<arr.length;i++)
            {
                arr[i].key = i;
                if(arr[i].year>=max_year)
                {
                    
                    if(arr[i].year == max_year && arr[i].half==1)
                    {
                        max_half=1
                    }
                    else if(arr[i].year > max_year )
                    {
                        max_half = arr[i].half
                    }
                    max_year = arr[i].year
                }
                if(arr[i].year<=min_year)
                {
                    if(arr[i].half==0)
                    {
                        min_half=0
                    }
                    min_year = arr[i].year
                }
            }
            var times = new Array();
            console.log("2");
            console.log(min_year,max_year,min_half,max_half);
            if(min_year==max_year)
            {
                if(min_half==max_half)
                {
                    times.push({year:min_year,half:min_half});
                }
                else
                {
                    times.push({year:min_year,half:0});
                    times.push({year:min_year,half:1});
                }
            }
            else
            {
                times.push({year:min_year,half:min_half})
                if(min_half == 0)
                {
                    times.push({year:min_year,half:1})
                }
                for(var i=min_year+1;i<max_year;i++)
                {
                    times.push({year:i,half:0});
                    times.push({year:i,half:1});
                }
                if(max_half == 0)
                {
                    times.push({year:max_year,half:0});
                }
                else
                {
                    times.push({year:max_year,half:0});
                    times.push({year:max_year,half:1});
                }
            }
            console.log("3");
            times.forEach(function(item,index,array){
                item.grades = [];
                arr.forEach(function(item1,index1,array1){
                    if(item1.year==item.year&&item1.half==item.half)
                    {
                        item.grades.push(item1);
                    }
                })
            });
            console.log(arr);
            that.setState({
                times:times,
                avg_grade_point : response.data.avg_grade_point,
                taked_credits : response.data.taked_credits,
                falied_credits : response.data.falied_credits,
                failed_num : response.data.failed_num,
                loading:false
            })
        })
        .catch(function (error) {
            
            if(isExpire(error))
                that.props.redirect();
            that.setState({
                has_grade:false,
                loading:false
            })
            //const arr = Object.keys(error);
            console.log(error);
            //that.forceUpdate();
        })
    }
    render(){
        var that = this;
        if(!this.state.has_grade)
        {
            return(<p>还尚未有成绩 </p>)
        }
        return (
            <Spin spinning={this.state.loading}>
            <div>
                <Row> 
                <Col  span={3} offset={0} >
                <b>总平均绩点:{this.state.avg_grade_point}</b>
                </Col>
                <Col  span={3} offset={2} >
                <b>已修学分:{this.state.taked_credits}</b>
                </Col>
                <Col  span={3} offset={2} >
                <b>不及格学分:{this.state.falied_credits}</b>
                </Col>
                <Col span={3} offset={2}>
                <b>不及格门数:{this.state.failed_num}</b>
                </Col>
                </Row>
                <br></br>
                <br></br>
            {
            this.state.times.reverse().map(function(item,key,array){
                let semester;
                if(item.half==0)
                {
                    semester="二"
                }
                else if(item.half==1)
                {
                    semester="一"
                }
                //{item.year+item.half-1}-{item.year+item.half}学年 第{semester}学期的成绩:
                var title = (item.year+item.half-1) +"-"+(item.year+item.half)+"学年 第"+semester+"学期";
                return (<div>
                <Table title={()=><b justify="center">{title}</b>} pagination={false} bordered dataSource={item.grades} columns={that.columns1} scroll={{ x: "90%" }}>
                    </Table><br/><br/></div>)
            })}
            </div>
            </Spin>
        );
    }

 
}

export default GetGrades;