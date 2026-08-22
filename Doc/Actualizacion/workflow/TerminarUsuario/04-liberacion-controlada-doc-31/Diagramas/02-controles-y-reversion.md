# Controles y reversión

```text
Solicitud autorizada
       │
       ▼
Verificación documental / SELECT sanitizado
       │
       ├─ continuar: versión y controles coinciden
       └─ abortar: falta autorización o hay discrepancia
                         │
                         ▼
     reversión por paquete previamente aprobado
       └─ solo afecta intentos nuevos
```
