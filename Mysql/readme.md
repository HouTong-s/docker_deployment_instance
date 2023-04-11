打包成镜像：docker build -t my-mysql-image:latest .
运行镜像：docker run -d -p 8086:3306 --network my-network --name my-mysql-container my-mysql-image:latest
运行之后，可以通过8806端口来访问数据库了
