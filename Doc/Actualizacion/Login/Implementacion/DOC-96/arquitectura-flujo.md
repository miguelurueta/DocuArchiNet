# Arquitectura y flujo técnico DOC-96

## Límites

La interfaz conserva el postback Web Forms. El navegador nunca envía empresa, módulo, usuario, correo, challenge o ruta de retorno al ASMX. La Session creada por `ClassGestorSesion.InicioAplicacionWebGestorDocumental` es la fuente de identidad y autorización.

```text
[Cliente / gestor.aspx]
        |
        | POST Web Forms: módulo + usuario + contraseña
        v
[gestor.Button1_Click]
        |
        v
[ClassGestorSesion.InicioAplicacionWebGestorDocumental]
        |
        +-- "YES" ------------------------> [navegación legacy sin cambio]
        |
        +-- error público -----------------> [mensaje legacy; no muestra OTP]
        |
        `-- "SECOND_FACTOR_REQUIRED"
                    |
                    v
       [DocuArchiSegundoFactor.iniciar()]
                    |
                    | POST ObtenerEstado {}
                    v
       [WebServiceLoginSegundoFactor.asmx]
                    |
          +---------+----------+
          |                    |
     NO_DISPONIBLE         PENDIENTE
          |                    |
   [formulario login]      [panel OTP]
                               |
             +-----------------+----------------+
             |                 |                |
     Verificar {codigo}    Reenviar {}      Cancelar {}
             |                 |                |
       PENDIENTE/          PENDIENTE/       NO_DISPONIBLE
       COMPLETADO          terminal             |
             |                 |          [restaurar login]
             v                 v
       [ruta local]       [actualizar/reset]
```

## Contrato consumido

| Operación | Entrada cliente | Retorno | Uso |
| --- | --- | --- | --- |
| `ObtenerEstado()` | `{}` | `SegundoFactorEstadoDto` | Restaurar Session activa al cargar o refrescar. |
| `Verificar(codigo As String)` | `{ "codigo": "…" }` | `SegundoFactorResultadoDto` | Validar el único dato permitido desde cliente. |
| `Reenviar()` | `{}` | `SegundoFactorResultadoDto` | Aplicar cooldown/límite autoritativos. |
| `Cancelar()` | `{}` | `SegundoFactorResultadoDto` | Eliminar el contexto y volver al login. |

El cliente exige el sobre JSON ASMX `{ d: ... }`. No se amplió el contrato.

## Estado cliente

`LOGIN → CONSULTANDO → PENDIENTE → SOLICITANDO → PENDIENTE|COMPLETADO|LOGIN`.

- Una bandera de exclusión mutua impide dos mutaciones simultáneas.
- Los segundos del servidor se convierten en marcas locales solo para representación.
- Al agotarse el tiempo, el cliente consulta `ObtenerEstado`; el reloj local no autentica ni habilita reenvío.
- Un resultado recuperable mantiene el panel; un terminal borra OTP y contraseña y restaura credenciales.

## Seguridad

- OTP exclusivamente en memoria y borrado al enviar/responder.
- Sin `localStorage`, `sessionStorage`, cookie propia, query string o logging de datos.
- Destino mostrado únicamente enmascarado por servidor.
- Navegación admitida solo para `SECOND_FACTOR_COMPLETED`, estado `COMPLETADO` y ruta `~/` sin esquema, host, traversal, barra inversa, query o fragmento.
- Errores de transporte/contrato se convierten en mensaje genérico y nunca en bypass.

## Accesibilidad

El panel usa título asociado, label explícito, `inputmode="numeric"`, `autocomplete="one-time-code"`, longitud seis, región viva, alertas, botones nativos y foco administrado. Los estilos proporcionan foco visible, disabled, contraste y adaptación móvil.

## Trazabilidad al código

| Responsabilidad | Archivo/símbolo |
| --- | --- |
| Bootstrap del pendiente | `gestor.aspx.vb:Button1_Click` |
| Markup accesible | `gestor.aspx#docuarchi-segundo-factor-panel` |
| Cliente/fábrica | `js/login/segundo-factor.js:createClient` |
| Transporte | `js/login/segundo-factor.js:createAsmxTransport` |
| Ruta segura | `isSafeLocalRoute`, `resolveLocalRoute` |
| Restauración WebForms | `createBrowserClient`, `PageRequestManager.endRequest` |
| Frontera servidor reutilizada | `webservice/WebServiceLoginSegundoFactor.asmx.vb` |
| DTO reutilizado | `DTOs/Login/SegundoFactor/SegundoFactorDtos.vb` |
