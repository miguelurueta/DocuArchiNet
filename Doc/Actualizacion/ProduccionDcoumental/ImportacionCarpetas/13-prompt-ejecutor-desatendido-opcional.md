# Prompt 13 — Ejecutor desatendido durable (opcional)

## Activación y dependencia

Esta entrega es una evolución separada de la primera implementación; no se ejecuta por completar 12. Requiere decisión explícita de incorporar procesamiento desatendido y definir dónde se alojará. Lee guía 00, diseño y componentes reales 07–12. La creación de este prompt no autoriza instalar servicios, desplegar ni modificar infraestructura.

## Encargo al activar esta entrega

Implementa ejecución durable para archivos ya recibidos, reutilizando el coordinador y sus contratos. No constituye una solución para leer archivos pendientes del dispositivo con el navegador cerrado.

## Trabajo

- Caracteriza dependencias restantes de HttpContext, Session, identidad, temporales y recursos locales. Un adaptador que simula sesión de usuario no satisface independencia del navegador. No modificar SII ni contratos compartidos para convertirlos en worker.
- Elige el host según infraestructura disponible y autorizada; documenta cuenta técnica, permisos mínimos, acceso a staging/gabinetes, arranque/parada y supervisión. No introducir una plataforma por preferencia sin necesidad.
- Recupera trabajo persistido y reclama fases atómicamente. Implementa señal de vida, revisiones y protección frente a dos ejecutores y frente al coordinador por peticiones. Definir quién es dueño del avance de cada lote; no mantener dos rutas competidoras sin exclusión.
- Autorización explícita: registra solicitante original y ejecutor técnico; revalida permisos y disponibilidad conforme al contrato. No usar cookies del usuario ni extender una sesión para ejecutar trabajos.
- Reiniciar después de caída/reciclado sin perder fases; reconciliar incertidumbre antes de repetir efectos. La exclusión expirada no basta para asegurar que un proceso anterior dejó de producir efectos.
- Mantener políticas completa/parcial, snapshot y nivel fijo del lote. Solo procesar contenido disponible y validado. Conservar detención segura, consulta de estado y acciones del usuario.
- Definir despliegue y retorno a modalidad anterior sin dos ejecutores activos y sin pérdida de trazabilidad. Entregar configuración de ejemplo sin secretos y SQL incremental MySQL 5.1 cuando aplique.

## Aceptación

En entorno autorizado demostrar continuidad tras cerrar navegador con archivos recibidos, reinicio del ejecutor, duplicación de instancias, pérdida de acceso al disco, permiso revocado y parada durante almacenamiento. Los archivos no recibidos siguen pendientes, sin acceso supuesto al disco local. No declarar completada esta evolución si los adaptadores aún requieren sesión del navegador; documentar la dependencia bloqueante y no ocultarla creando HttpContext ficticio.
