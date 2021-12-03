import argparse
import subprocess

down_cmd = "docker-compose down"
up_cmd = "docker-compose up -d"
rm_cmd = "docker rm"
rmi_cmd = "docker rmi -f"
rm_all_container_cmd = "docker rm -f $(docker ps -aq)"
rm_all_images_cmd = "docker rmi -f $(docker images -aq)"


def main():
    parse = argparse.ArgumentParser()
    parse.add_argument("--rebuild", help="Rebuild and restart a service")
    parse.add_argument(
        "--restart", help="Docker-compose down and up", action="store_true"
    )
    parse.add_argument("--down", help="Docker-compose down", action="store_true")
    parse.add_argument("--up", help="Docker-compose up", action="store_true")
    parse.add_argument(
        "-a", "--all", help="Rebuild and restart all service", action="store_true"
    )
    args = parse.parse_args()

    if args.up:
        subprocess.call(up_cmd.split())
        return
    if args.down:
        subprocess.call(down_cmd.split())
        return
    if args.restart:
        subprocess.call(down_cmd.split())
        subprocess.call(up_cmd.split())
        return

    # docker rmi <container_name>
    if args.rebuild:
        # docker-compose down
        subprocess.call(rm_cmd.split().extend(args.rebuild))
        subprocess.call(rmi_cmd.split().extend(args.rebuild))

    # Remove all containers and images
    if args.all:
        subprocess.call(rm_all_container_cmd.split())
        subprocess.call(rm_all_images_cmd.split())

    # docker-compose up -d
    subprocess.call(up_cmd.split())


if __name__ == "__main__":
    main()
