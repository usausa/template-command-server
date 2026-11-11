# Template project for TCP command server

[Template](https://usausa.github.io/template/) project for TCP command server.

# ✅ Implement

| Category | Feature |
| --- | --- |
| Server | TCP server (Mofucat.TcpService) / Line protocol (Pipelines) / Command handlers / Read timeout / Line length limit |
| Auth | Challenge-response (RSA signature) / Anonymous mode |
| Job | Cron job (Mofucat.JobScheduler) |
| Observability | Serilog / Startup log / Health checks / Metrics (OpenTelemetry, Prometheus) / Stats command |
| Basic | Setting / DI (source generator) |
| Operation | Windows service / systemd |

# 🔗 Related Projects

| Project | Description |
| --- | --- |
| [template-command-tool](https://github.com/usausa/template-command-tool) | CLI tool template |
| [template-worker](https://github.com/usausa/template-worker) | Worker Service template |

# 📦 My Libraries

| Library | Description |
| --- | --- |
| [Usa.Smart.Mapper](https://github.com/usausa/Smart-Net-Mapper) | Object mapper |
| [Mofucat.TcpService](https://github.com/usausa/mofucat-tcp-server) | Kestrel based TCP server |
| [Mofucat.JobScheduler](https://github.com/usausa/mofucat-job-scheduler) | Job scheduler |
| [BunnyTail.DependencyInjection](https://github.com/usausa/bunnytail-dependency-injection) | Source generator based DI |
