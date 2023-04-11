/**
 * @name: Simple
 * @author: wangjiao
 * @description：极简版本
 */
 import React from 'react';
 import styles from './table.less';
  
 const sections = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12].map((i) => `第${i}节`);
 const weeks = [0, "一", "二", "三", "四", "五", "六", "日"].map((i) => "周"+i );
/*
public int ScheduleId { get; set; }
public int TeacherId { get; set; }
public int? BeginWeek { get; set; }
public int? EndWeek { get; set; }
public string Place { get; set; }
public int? CurrentNum { get; set; }
public int? MaxNum { get; set; }
public string Note { get; set; }

public string Type { get; set; }
public string TeachingMaterial { get; set; }
public string Campus { get; set; }
public List<ScheduleTime> times { get; set; }
public string teacher_name { get; set; }
public string lesson_name { get; set; }
public decimal credit { get; set; }
*/

class TimeTable extends React.Component {
    constructor(props)
    {
        super(props);
    }
    render(){
        var data = this.props.schedules;
        console.log(data);
        if(data==null)
            return(<div></div>)
        return (
             <div className={styles.root}>
               {/*空课程表前端代码*/}
               <div className={styles.content}>
                 <div className={styles.rowHead}>
                   {
                     weeks.map((item, index) => {
                       if (index === 0) {
                         return (<div className={styles.empty} />)
                       }
                       return (<div className={styles.headItem}>{item}</div>)
                     })
                   }
                 </div>
                 {sections.map((rowItem, i) => <div className={styles.rowItem}>
                   {
                     weeks.map((columnItem, j) => {
          
                       if (j === 0) {
                         return <div className={styles.columnHead}>{rowItem}</div>
                       }
                       return <div className={styles.columnItem}/>
                     })
                   }
                 </div>)}
               </div>
               {/*课程名称代码*/}
               <div className={styles.topContent}>
                    {
                      data.map(function(item,index,array){
                          //var str = item.teacher_name+"("+item.teacherId+") "+item.lesson_name+"("+item.scheduleId+") "+"["+item.beginWeek+"-"+item.endWeek+"]";
                          var str = item.lesson_name;
                          return item.times.map(function(item1,index1,array1){
                            var temp1 = "";
                            if(item1.singleOrDouble == 1) 
                            {
                                temp1 = "(单)";
                            }
                            else if(item1.singleOrDouble == 2)
                            {
                                temp1 = "(双)";
                            }
                            return<div className={styles.item} style={{top:`${(item1.beginSection-1)*52}px`,
                            left:`${(item1.dayWeek-1)*140}px`,height:`${(item1.endSection-item1.beginSection+1)*52}px`,lineHeight:`${(item1.endSection-item1.beginSection+1)*52}px`}} >{str+temp1}</div>  
                          })
                          
                      })
                      /* (item)=>{
                          return<div className={styles.item} style={{top:`${(item.startSection-1)*52}px`,
                            left:`${(item.week-1)*100}px`,height:`${(item.endSection-item.startSection+1)*52}px`,lineHeight:`${(item.endSection-item.startSection+1)*52}px`}} >{item.course}</div>
                      } */
                    }
               </div>
             </div>
           )
    } 
}
export default TimeTable;
  