import argparse
import multiprocessing
import subprocess

black_cmd = "black run.py"
csharpier_cmd = "dotnet csharpier ."
api_run_cmd = "dapr run --app-id EPlusActivities --app-port 52537 --dapr-http-port 3500 --components-path Dapr/Components-dev -- dotnet watch run --project EPlusActivities.API"
api_watch_run_cmd = "dapr run --app-id EPlusActivities --app-port 52537 --dapr-http-port 3500 --components-path Dapr/Components-dev -- dotnet run --project EPlusActivities.API"
file_service_run_cmd = "dapr run --app-id FileService --app-port 52500 --app-protocol grpc --components-path Dapr/Components-dev -- dotnet watch run --project FileService"
file_service_watch_run_cmd = "dapr run --app-id FileService --app-port 52500 --app-protocol grpc --components-path Dapr/Components-dev -- dotnet run --project FileService"

format_cmds = [black_cmd, csharpier_cmd]
run_cmds = [api_run_cmd, file_service_run_cmd]
watch_run_cmds = [api_watch_run_cmd, file_service_watch_run_cmd]


def main():
    parse = argparse.ArgumentParser()
    parse.add_argument("-w", "--watch", help="dotnet watch run", action="store_true")
    args = parse.parse_args()

    map(subprocess.call, [cmd.split() for cmd in format_cmds])

    if args.watch:
        map(
            lambda x: x.start(),
            [subprocess.call(cmd) for cmd in [x.split() for x in run_cmds]],
        )
    else:
        map(
            lambda x: x.start(),
            [subprocess.call(cmd) for cmd in [x.split() for x in watch_run_cmds]],
        )


if __name__ == "__main__":
    main()
