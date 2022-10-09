# MetaNet
![Test result](https://img.shields.io/badge/Windows-passing-green)
![GitHub License](https://img.shields.io/github/license/linkmeta/MetaNet?color=blue&style=flat-square)
![Issues](https://img.shields.io/github/issues/linkmeta/MetaNet?color=blue&style=flat-square)
![release](https://img.shields.io/github/release/linkmeta/MetaNet.svg)

![logo](/Resources/favicon.png)

MetaNet is a network toolkit, support iperf,tcp,udp,websocket,mqtt,sniffer,pcap,port scan,listen,ip scan .etc.

This is an easy demo for network study by WPF MVVM.

## Function

- [x] Iperf2/Iperf3 Server and Client(Realtime plot and result save)
- [x] Network Scan(IP multi-thread scan)
- [x] Port Scan(Port status multi-thread scan)
- [x] Route tables(IPv4 & IPv6)
- [x] Local Port listen(TCP & UDP)
- [x] TCP Server & Client
- [x] UDP Server & Client
- [x] WebSocket Server & Client
- [ ] MQTT
- [x] Sniffer(Using pcap, packets capture, save, filter, statistics, plot. etc.)
- [ ] ...
<img src="https://github.com/linkmeta/MetaNet/blob/main/demo/iperf.png" width="700">
<img src="https://github.com/linkmeta/MetaNet/blob/main/demo/sniffer.png" width="700">
<img src="https://github.com/linkmeta/MetaNet/blob/main/demo/snifferstats.png" width="700">
<img src="https://github.com/linkmeta/MetaNet/blob/main/demo/websocket.png" width="700">
<img src="https://github.com/linkmeta/MetaNet/blob/main/demo/portlisten.png" width="700">
<img src="https://github.com/linkmeta/MetaNet/blob/main/demo/routetable.png" width="700">


## Build

- [x] VisualStudio 2022（Based .NET WPF）

```bash
$ git clone https://github.com/linkmeta/MetaNet.git
$ cd MetaNet
```
Run `MetaNet.sln`

## Packages
```
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="OxyPlot.Core" version="2.1.0" targetFramework="net48" />
  <package id="OxyPlot.Wpf" version="2.1.0" targetFramework="net48" />
  <package id="OxyPlot.Wpf.Shared" version="2.1.0" targetFramework="net48" />
  <package id="Pcap.Net.x64" version="1.0.4.1" targetFramework="net48" />
  <package id="Pcap.Net.x86" version="1.0.4.1" targetFramework="net48" />
  <package id="System.Globalization.Extensions" version="4.3.0" targetFramework="net48" />
  <package id="WebSocketSharp" version="1.0.3-rc11" targetFramework="net48" />
</packages>
```
## Contribute

* [Iperf](https://github.com/esnet/iperf)
* [oxyplot](https://github.com/oxyplot/oxyplot)

## License

MIT License（[License MIT](./LICENSE)）
