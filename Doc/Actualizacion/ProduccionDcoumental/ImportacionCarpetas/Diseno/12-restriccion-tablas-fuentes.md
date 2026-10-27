# DOC-110 — Restricción de tablas candidatas y consultables

Solicitud del usuario: por seguridad, mostrar solo tablas con prefijo imp_02 y permitir excepciones configuradas explícitamente. Fecha: 2026-10-23.

## Regla vigente en el código

En RA y DA se admite por defecto únicamente un nombre que empiece literalmente por `imp_02`, con comparación ordinal sensible a mayúsculas. El guion bajo no funciona como comodín. `imp_02_egresos` está permitido; `impX02_egresos`, `usuarios` o `IMP_02_egresos` no. Se mantienen validación de identificadores y aislamiento por conexión/empresa.

RestriccionTablasFuente es un límite del servidor independiente de administrar fuentes. El catálogo RA sigue guardando autorizaciones funcionales y columnas. Cumplir el prefijo o tener excepción no registra ni activa automáticamente una fuente. Una fuente fuera de este límite deja de ofrecerse a proveedores y no puede consultarse, aunque su autorización siga persistida; se conserva su historial administrativo para revisión/revocación.

Controles implementados:

- TABLAS filtra en information_schema antes de LIMIT/OFFSET mediante parámetros y comparación BINARY. La paginación no expone nombres de otras tablas ni termina anticipadamente por filtrado posterior.
- COLUMNAS rechaza una tabla fuera de alcance antes de resolver/abrir conexión; no basta enviar manualmente un nombre a ASMX.
- Alta, edición y activación validan mediante el mismo descubrimiento restringido. Revocar sigue disponible para retirar autorizaciones anteriores.
- La política de proveedores filtra fuentes RA con la misma restricción; la tabla no puede eludirla mediante un ID de proveedor ya guardado.
- La interfaz informa del alcance. El control no depende de JavaScript.

## Excepciones de servidor

Clave opcional de appSettings en Web.config: `Doc110.Fuentes.TablasAdicionales`. Ausente o vacía equivale a ninguna excepción. Formato: arreglo JSON cerrado, hasta 64 entradas únicas por conexión/nombre. Solo alias RA/DA y nombres físicos exactos válidos; sin comodines, SQL, credenciales ni prefijos libres.

Ejemplo ilustrativo para autorizar una tabla adicional únicamente en DA (no aplicado):

```xml
<add key="Doc110.Fuentes.TablasAdicionales"
     value='[{"ConexionRef":"DA","Tabla":"tabla_adicional_aprobada"}]' />
```

No duplicar appSettings ni la clave si ya existe. La excepción es configuración de seguridad del servidor; no es un campo editable por quien crea proveedores. Para retirarla, eliminar esa entrada: una nueva petición ya no puede utilizar esa tabla. Configuración inválida se rechaza; nunca amplía acceso a todas las tablas.

No se reintroduce la antigua lista `Doc110.Proveedores.FuentesAutorizadas`: el registro funcional sigue en RA. Esta nueva clave solo limita qué nombres pueden autorizarse. No se modificó Web.config en este turno ni se habilitaron excepciones.

## Evidencia

- Harness .NET existente PASS: prefijo literal, nombres ajenos/mayúsculas/comodines rechazados, excepción exacta limitada a DA, fuente persistida fuera de alcance excluida y solicitud directa de COLUMNAS denegada antes de conectar.
- Build aislado PASS en .tmp/doc110-amp02-build; sin copia a bin ni despliegue.
- Chromium doc110-fuentes-guided.spec.cjs: 1 PASS con transporte simulado; no acredita rechazo HTTP real en IIS.
- SELECT real de information_schema en DA, 2026-10-23T13:54:27.7419883Z: filtro parametrizado previo a LIMIT devolvió 23 nombres, todos con imp_02, incluido imp_02_egresos. No se consultaron filas de negocio ni se modificaron tablas, permisos o datos.

La prueba real contra la aplicación desplegada permanece pendiente. SII no se modificó; una tabla cuyo nombre coincida con el prefijo no fue leída ni escrita por esta comprobación de metadatos.
