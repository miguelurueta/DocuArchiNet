# Diseño 02 — Contratos de operación

Estado: contrato propuesto para implementar tras cerrar pendientes de [04](04-decisiones-y-pendientes.md). Nombres de operaciones propios; no son endpoints existentes. El diseño se adapta a WebForms/ASMX/ASHX, no requiere MVC.

## 1. Transporte y autoridad

Propuesta: ASMX nuevo para control JSON y ASHX nuevo para contenido multipart, fuera del handler compartido. Los nombres físicos se fijan siguiendo organización del proyecto al implementar. No modificar servicios SII ni añadir ramas de lote al handler de carga individual. Adaptadores de dominio no se exponen directamente a HTTP.

ASMX devuelve DTO tipado: el cliente desempaqueta `d` si corresponde, sin doble serialización ni JSON como cadena dentro de JSON. ASHX devuelve el mismo sobre lógico sin `d`. El adaptador HTTP de frontend oculta esa diferencia. Toda mutación requiere protección contra solicitudes falsificadas compatible con la aplicación; incluirla también en multipart. No inventar cookies, reutilizar autenticación existente y resolver usuario/ámbito efectivo en servidor.

IDs UID/legacy y cantidades de bytes se representan como cadenas canónicas para no perder precisión en JavaScript. Revisiones y contadores acotados se representan como enteros. UTC en formato ISO 8601 con Z. Hash en hexadecimal minúscula de 64 caracteres; JSON no envía bytes del contenido. El request no puede incluir nombres SQL ni ruta física de staging.

Sobre común:

```json
{
  "ok": false,
  "codigo": "PLAN_NO_CONFIRMABLE",
  "mensaje": "Revisa las incidencias indicadas.",
  "operacionId": "uid-servidor",
  "datos": null,
  "incidencias": [
    {
      "ambito": "ARCHIVO",
      "raizIdCliente": "r1",
      "archivoIdCliente": "a1",
      "campo": "tipologia",
      "codigo": "TIPOLOGIA_NO_RESUELTA",
      "mensaje": "No se pudo resolver la tipología.",
      "bloqueaUnidad": true
    }
  ]
}
```

Los identificadores del ejemplo son simbólicos; las implementaciones validan el formato definido. HTTP 200 no significa documento incorporado; el sobre y estado son autoridad. Preferir 400 estructura, 401 sin autenticación, 403 no autorizado, 409 revisión/conflicto, 413 límites y 500/503 error interno o capacidad no disponible. Si ASMX/IIS transforma la respuesta, probar el comportamiento y mapearlo en el cliente, sin mostrar HTML/stack trace. Error de transporte después de mutación requiere consulta, no reenvío ciego.

## 2. Operaciones y propietarios

| Operación | Propietario | Entrada principal | Salida / efecto |
|---|---|---|---|
| AbrirImportacion | 06 | nivelRef del árbol | Contexto validado, ruta canónica, perfiles compatibles y límites. Sin efectos documentales. |
| Listar/ObtenerPerfil | 02 | versión/perfil y referencia de contexto | Configuración autorizada, estado y compatibilidad. |
| PrepararPlan | 06 | nivelRef, versionId, inventario completo | planId/revisión/huella, incidencias, unidades elegibles y valores. Guarda preparación, no expedientes. |
| ConfirmarPlan | 07-B | planId, revisión, huella y claveConfirmacion | loteId, raíces/archivos persistidos y política efectiva. Transacción idempotente. |
| RecibirArchivo | 07-B | loteId, archivoId, clave de intento y un archivo multipart | recepción confirmada/rechazada, bytes/huella verificados; nunca éxito documental. |
| AvanzarLote | 10 | loteId y revisión/clave de comando | Una fase acotada y estado actual. No promete ejecución posterior autónoma. |
| SolicitarDetencion | 10 | loteId, revisión y clave | Señal registrada y operaciones en vuelo conocidas. No rollback. |
| ReconciliarRecurso | 10 | loteId, recursoId, revisión y motivo | Verificación con evidencia o incertidumbre preservada. Puede completar fase pendiente solo si explícitamente autorizada. |
| ConsultarLote / ListarLotes | 11 | loteId o filtros acotados/cursor | Estado persistido, totales, errores y acciones permitidas. Sin efectos. |
| ConsultarArchivos/Raices | 11 | loteId, filtros y cursor | Páginas ordenadas y revisión de lectura. |
| ExportarResultados | 11 | loteId/filtros autorizados | CSV protegido; no descarga archivos originales. |

El cierre de ventana es local, no endpoint de cancelar. Una preparación nueva no elimina un lote anterior. El contexto enviado nunca puede sustituir la identidad autenticada.

## 3. Apertura desde Producción

`AbrirImportacion(nivelRef)` comprueba: identidad, ámbito derivado, existencia, elegibilidad del nivel y capacidad de crear/importar. Devuelve nivelRef/ruta, referencias de clasificación contextual si aplican, versiones de catálogo relevantes y permisos por acción. No crea niveles.

La ruta de display no identifica el nivel. Si no hay sello de revisión legacy, conservar en el plan una huella de la cadena de ancestros/ámbito relevante y reconsultar antes de efectos. No mantener un token duradero que reemplace revalidación de permisos. Otra pestaña puede navegar sin modificar el contexto del plan.

## 4. Preparación y canonización

Primera implementación: una solicitud completa de manifiesto, limitada por cantidad/bytes configurados y límites HTTP. Si el volumen aprobado requiere fragmentación, debe añadirse un contrato explícito de preparación por fragmentos con completitud; no implementarlo parcialmente ni anunciar una capacidad inexistente. La configuración de límites es requisito para habilitar la capacidad.

```json
{
  "nivelRef": "referencia-verificada-del-arbol",
  "versionId": "uid-publicado",
  "raices": [{"idCliente":"r1","nombre":"1246566"}],
  "archivos": [{
    "idCliente":"a1","raizIdCliente":"r1",
    "nombre":"1246566-125-OrdenCompra.pdf",
    "rutaRelativa":"1246566/1246566-125-OrdenCompra.pdf",
    "bytes":"1450020","ultimaModificacionLocal":"2026-10-19T10:00:00Z"
  }]
}
```

Comparar nombre con último segmento, raíz con primer segmento; aceptar únicamente rutas relativas normalizadas con `/`, rechazando absolutos, `..`, vacíos ambiguos y separadores peligrosos. Las raíces tienen identidad independiente de nombre. Canonización ordena por IDs estables, claves de objetos según esquema, representación estricta de números/cadenas y reglas de Unicode documentadas. No aplicar cambios destructivos al nombre original. Reordenar una tabla visual no cambia identidad; cambiar inventario/configuración genera nueva preparación/planId.

Resultado incluye:

- `planId`, `revision`, `planHash`, `expiraUtc`, versión/configuración efectiva y nivel validado.
- Política de integridad y ámbito configurado: EXPEDIENTE o LOTE para modo completo, NO_APLICA para parcial; nunca un default implícito.
- `puedeConfirmar`, `erroresGlobales`, `unidadesElegibles`, `unidadesBloqueadas` y totales originales.
- Cada raíz: nombre visible propuesto, fuentes del código/fechas, estado y conflictos.
- Cada archivo: valores resueltos, valores diferidos con fuente/etapa/tipo, tipología e incidencias. No bytes ni supuesta validación de contenido.

En parcial, una raíz sin ningún archivo elegible no crea expediente vacío. Errores globales bloquean siempre. En completo por EXPEDIENTE, una raíz sin errores previos puede ser elegible aunque otra esté bloqueada; conservar todas las incidencias y el inventario. En completo por LOTE, cualquier error impeditivo de un archivo bloquea la confirmación global. Tras confirmar, ejecutar solo cuando todos los archivos de la unidad exigida estén recibidos y validados. `puedeConfirmar` no significa que los archivos ya estén recibidos.

## 5. Confirmación e idempotencia

Entrada: planId, revisión/huella esperadas y claveConfirmacion estable creada para esa confirmación. El servidor carga el plan, no acepta valores finales del cliente. Revalida disponibilidad/autoridad/destino y coherencia; un cambio que altera resultados exige otro plan y revisión del usuario.

Transacción: comprobar/reclamar plan → resolver clave existente → crear lote y todas sus raíces/archivos → asociar snapshot y marcar plan CONFIRMADO → commit. Las restricciones únicas evitan dos lotes bajo otra clave para el mismo plan. Ante repetición idéntica autorizada devolver lote existente; mismo key/datos distintos: conflicto. No considerar expirado el reintento de confirmación de un lote ya creado, pero revalidar acceso para consultarlo.

Salida contiene loteId y mapping `clienteRaizId→raizId`, `clienteArchivoId→archivoId`; archivos rechazados se conservan identificados y sin autorización de recepción según política. Totales originales no disminuyen por rechazo.

## 6. Recepción

Un archivo por solicitud. Referencias de lote/archivo y clave de intento via metadatos multipart; contenido como binario. Validar pertenencia antes de aceptar y durante transferencia aplicar cuotas concurrentes. No admitir ruta de escritura proporcionada por cliente.

Proceso: reclamar recepción → escribir parte bajo raíz controlada → comprobar tamaño/contenido permitidos y SHA-256 → cerrar archivo → convertirlo en staging completo → persistir referencia/huella/estado. El orden no crea transacción disco/DB: registrar evidencia y reconciliar crash en cada frontera. Si hay escritura completa sin registro, no procesar como documento hasta verificar y asociar autorizado; si fila dice RECIBIDO pero archivo falta, bloquear efectos y solicitar recuperación segura.

Salida `RECIBIDO` con bytes/huella significa únicamente staging validado. Reintento del mismo contenido recupera resultado; contenido diferente no sustituye silenciosamente un archivo confirmado. Nombre/tamaño/fecha de reselección no prueban identidad, aunque ayudan a correlacionar candidatos.

## 7. Puertos internos

| Puerto propuesto | Contrato |
|---|---|
| PerfilConsulta / ResolucionCampos | Devuelve versión/configuración autorizada y valores tipados/resoluciones diferidas. No crea documentos. |
| ContextoProduccion | Valida identidad de nivel, permisos, ámbito y ruta actual. Reusable por apertura/confirmación/ejecución. |
| PlanRepositorio / OperacionRepositorio | Persistencia, CAS, reclamos e idempotencia. No reglas de nombres ni HTML. |
| ArchivoStaging | Recepción/referencia controlada, validación/huella y copia de trabajo. No camino arbitrario. |
| ExpedienteAdapter | Comando tipado, IDs reales/relación, rechazo conocido o resultado incierto. |
| DocumentoAdapter | Almacenar/verificar con campos finales, formato y contexto. Resultado distingue fase e IDs conocidos. |
| Reconciliacion | Consulta evidencia por operación; no inferir éxito solo por nombre/huella global. |

Los contratos no fuerzan clases concretas o interfaces del código SII. Resultado de adaptador: `Confirmado`, `RechazadoSinEfectos`, `ParcialConocido`, `Incierto`; incluye fase, IDs conocidos y evidencia saneada. Una excepción/retorno distinto de YES en legacy no demuestra ausencia de efecto.

## 8. Consulta y recuperación

Propuesta de paginación por cursor opaco ligado a lote/filtro, orden estable por raíz/archivo ID y límite configurado. Reportar revisión de lectura; el usuario puede refrescar si el lote avanza, sin afirmar snapshot global transaccional de páginas separadas. Consulta no ejecuta trabajo.

`accionesPermitidas` se calcula con estado/autoridad: continuar, detener, reseleccionar pendientes, solicitar verificación. La UI no decide habilitación solo por color/etiqueta. Reconciliar requiere permiso distinto de consultar; si recupera un ID, debe probar el vínculo operación–efecto y destino. Ajustes manuales de resultado exigen evidencia auditada, no un parámetro `marcarExitoso=true`.

## 9. Códigos de error mínimos

| Código | Tratamiento |
|---|---|
| NO_AUTENTICADO / NO_AUTORIZADO | No iniciar efectos; renovar acceso o rechazar. |
| DESTINO_INVALIDO / DESTINO_CAMBIO | Bloquear unidad afectada; nueva revisión sin redirección. |
| PERFIL_NO_DISPONIBLE / VERSION_NO_COMPATIBLE | No sustituir por última versión; revisar. |
| MANIFIESTO_INVALIDO / LIMITE_EXCEDIDO | Rechazo estructural/global, no parcial silencioso. |
| PLAN_EXPIRADO / REVISION_OBSOLETA | Preparar/revisar de nuevo salvo confirmación ya persistida consultable. |
| IDEMPOTENCIA_CONFLICTO | Misma clave con contenido distinto; no repetir efecto. |
| ARCHIVO_RECHAZADO / CONTENIDO_DISTINTO | Incidencia por elemento con política aplicada. |
| OPERACION_EN_CURSO | Consultar; no lanzar un competidor. |
| RESULTADO_INCIERTO | Verificar, no reenviar almacenamiento automáticamente. |
| ESQUEMA_NO_DISPONIBLE | Solo capacidad nueva no disponible; carga individual sigue operativa. |

El prompt 12 debe probar serialización real ASMX/ASHX, status HTTP, sesión expirada y CSRF; este documento no certifica que el pipeline actual ya los resuelva.
