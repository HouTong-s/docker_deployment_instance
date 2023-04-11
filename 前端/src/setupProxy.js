//新版配置代码
const {createProxyMiddleware} = require('http-proxy-middleware')

module.exports = function(app){
  app.use(
    createProxyMiddleware('/api',{
      target:'http://localhost:8000/',
      changeOrigin:true,
      pathRewrite:{'^/api':'/api'},//地址重写，这里可以不要，最后还是/api的形式
      secure: false//不验证证书，必须加上，不然会出问题
    })
    //http://139.224.50.124:8000/
    //http://180.76.54.64:8000/
    //http://localhost:8000/
  )
}