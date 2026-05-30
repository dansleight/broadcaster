
## Installing Everything
Assuming that we've already run this indev, so we have the dotnet SDK that we need, as well as `Node` and `pnpm`. It's worth doing an update and upgrade, and make sure we have `nginx` installed:
```bash
sudo apt install -y nginx
```

## Build and Publish.
Switch to the `spa-src` directory and build the SPA into the WebAPI app:

```bash
cd spa-src
pnpm i
pnpm run build:release
```

Then switch to the WebAPI Project directory, and publish:
```bash
cd ../src/Broadcaster
sudo dotnet publish -c Release -o /var/www/broadcaster --no-restore
```

## Setup the directory and permissions
It's not good to run this as root, so we need a user to run as:

```bash
sudo adduser --system --group --no-create-home --shell /usr/sbin/nologin broadcastuser

sudo chown -R broadcastuser:broadcastuser /var/www/broadcaster
sudo chown -R 755 /var/www/broadcaster
```

## Create service
```bash
sudo nano /etc/systemd/system/broadcaster.service
```

Then something like:
```
[Unit]
Description=Broadcaster App
After=network.target

[Service]
Type=simple
User=broadcastuser
WorkingDirectory=/var/www/broadcaster
ExecStart=/usr/bin/dotnet /var/www/broadcaster/Broadcaster.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=broadcaster
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:8001
Environment=EntraId__TenantId={Entra Tenant Id}
Environment=EntraId__ClientId={Entra Client Id}
Environment=EntraId__Domain={Entra Domain}
Environment=EntraId__WebApiScope={Entra Scope}
Environment=Broadcast__RtmpUri=rtmps://a.rtmps.youtube.com/live2/{youtube broadcast key}
# If you created a .env file:
# EnvironmentFile=/var/www/myapp/.env

[Install]
WantedBy=multi-user.target
```

Then reload the daemon, setup and start the service:
```bash
sudo systemctl daemon-reload
sudo systemctl enable broadcaster
sudo systemctl start broadcaster
```

Check it:
```bash
sudo systemctl status broadcaster
sudo journalctl -u broadcaster -f # live logs
```

