import {message} from 'antd';
function isExpire(error)
{
    console.log(error);
    console.log(error.response);
    if(error.response.data.code != null && error.response.data.code == 401)
    {
        localStorage.removeItem("token");
        message.error(error.response.data.msg);
        return true;     
    }
}
export {isExpire};