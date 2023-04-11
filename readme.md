前置条件：安装docker，并创建docker网络：docker network create my-network
然后让两个容器A、B都连接这个网络：docker run -d --network my-network --name A image-A
docker run -d --network my-network --name B image-B

在容器A中，您可以使用容器B的名字来访问它提供的服务，而不需要知道它的IP地址。例如，如果您想要使用curl命令来访问B的8080端口，您可以这样做：
docker exec -it A curl http://B:8080
也就是说把host(主机名)改为B容器的名字就可以了