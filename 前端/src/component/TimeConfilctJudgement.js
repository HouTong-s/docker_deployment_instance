function IsConflict1(target,list)
{
    for(var i=0;i<list.length;i++)
    {
        if(list[i].beginWeek > target.endWeek || list[i].endWeek <target.beginWeek)//周之间没有交集，则必定不冲突
        {
            continue;
        }
        else if(!IsConflict2(target.times,list[i].times))//每次单个的时间是否冲突
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
function IsConflict2(target,list)
{
    for(var i=0;i<target.length;i++)
    {
        if(IsConflict3(target[i],list))
            //其中一个冲突整体就冲突,否则继续遍历
        {
            return true;
        }
    }
    return false;
}
function IsConflict3(target,list)
{
    for(var i=0;i<list.length;i++)
    {
        if( (list[i].singleOrDouble == 1 && target.singleOrDouble == 2 ) ||((list[i].singleOrDouble == 2 && target.singleOrDouble == 1)))
            //一个单周一个双周肯定不冲突
        {
            continue;
        }
        else
            /*单双周模式相同，则只用比较每一天是否冲突
                一单一全和一双一全同理
                */
        {
            if(list[i].dayWeek != target.dayWeek)
                //不在同一个星期几上课，必然不冲突
            {
                continue;
            }
            else if(list[i].endSection < target.beginSection || list[i].beginSection >target.endSection)
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
export {IsConflict1};