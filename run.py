import argparse
import multiprocessing
import subprocess

api_run_cmd = "dapr run --app-id EPlusActivities --app-port 52537 --dapr-http-port 3500 --components-path Dapr/Components-dev -- dotnet watch run --project EPlusActivities.API"
api_watch_run_cmd = "dapr run --app-id EPlusActivities --app-port 52537 --dapr-http-port 3500 --components-path Dapr/Components-dev -- dotnet run --project EPlusActivities.API"
file_service_run_cmd = "dapr run --app-id FileService --app-port 52500 --app-protocol grpc --components-path Dapr/Components-dev -- dotnet watch run --project FileService"
file_service_watch_run_cmd = "dapr run --app-id FileService --app-port 52500 --app-protocol grpc --components-path Dapr/Components-dev -- dotnet run --project FileService"
run_cmds = [api_run_cmd, file_service_run_cmd]
watch_run_cmds = [api_watch_run_cmd, file_service_watch_run_cmd]


def main():
    parse = argparse.ArgumentParser()
    parse.add_argument("-w", "--watch", help="dotnet watch run", action="store_true")
    args = parse.parse_args()

    if args.watch:
        for cmd in run_cmds:
            multiprocessing.Process(
                target=subprocess.call,
                args=(cmd.split(),),
            ).start()
    else:
        for cmd in watch_run_cmds:
            multiprocessing.Process(
                target=subprocess.call,
                args=(cmd.split(),),
            ).start()


if __name__ == "__main__":
    main()
