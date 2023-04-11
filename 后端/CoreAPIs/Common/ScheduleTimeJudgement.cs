using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPIs.Models;
namespace CoreAPIs.Common
{
    public class ScheduleTimeJudgement
    {
        public static bool IsConflict(ScheduleTimes_Info target,List<ScheduleTimes_Info> list)
        {
            foreach(var info in list)
            {
                if(info.begin_week > target.end_week || info.end_week <target.begin_week)//周之间没有交集，则必定不冲突
                {
                    continue;
                }
                else if(!IsConflict(target.times,info.times))//每次单个的时间是否冲突
                {
                    continue;
                }
                else//否则肯定冲突了，直接返回
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsConflict(List<SingleTime> target, List<SingleTime> list)
        {
            foreach(var info in target)
            {
                if(IsConflict(info,list))
                    //其中一个冲突整体就冲突,否则继续遍历
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsConflict(SingleTime target, List<SingleTime> list)
        {
            foreach(var info in list)
            {
                if( (info.single_or_double == 1 && target.single_or_double == 2 ) ||((info.single_or_double == 2 && target.single_or_double == 1)))
                    //一个单周一个双周肯定不冲突
                {
                    continue;
                }
                else
                    /*单双周模式相同，则只用比较每一天是否冲突
                      一单一全和一双一全同理
                        */
                {
                    if(info.dayweek != target.dayweek)
                        //不在同一个星期几上课，必然不冲突
                    {
                        continue;
                    }
                    else if(info.end_section < target.begin_section || info.begin_section >target.end_section)
                        //这样也不冲突
                    {
                        continue;
                    }
                    //其余情况必然冲突了
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
