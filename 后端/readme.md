后端采用.NET5(.NET6镜像也可以运行.NET5)
构建镜像：docker build -t my_core_api .
运行：docker run -d -p 8087:8000 -p 8090:80 --network my-network --name dotnet_api my_core_api
找到图像文件：docker cp 0d78c1377a21176c2f53fa9b5ae47e0f8b96226277c82c199bce0873474ea253:/app/imgs/yukji.jpg .