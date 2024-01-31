# Redis demo

## Run Redis in docker
```
docker run --name redis1 -p 5002:6379 -d redis
```

## Run Redis interactively
```
docker exec -it redis1 sh
```

## Type redis-cli
![Screenshot](screenshot-docker-exec.png)