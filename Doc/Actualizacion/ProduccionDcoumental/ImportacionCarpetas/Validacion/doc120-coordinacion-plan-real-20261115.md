# DOC-120 — ventana propuesta para aceptar la coordinación

Ampliación documental2026-11-17: [espera y reanudación general](doc120-reanudacion-plan-real-20261117.md) propone una ventana separada de un control/un documento del lote existente. No amplía los efectos ni autorizaciones de este plan; no se ejecutó.

Estado: **PREPARADA, SIN AUTORIZACIÓN NI EJECUCIÓN**. Tareas3.86/3.87 pendientes. La aprobación de implementación habilitó código y pruebas locales; no identifica las cuentas ni autoriza los efectos siguientes. Aplicar [AGENT-RUNBOOK.md](../../../../../tools/e2e/AGENT-RUNBOOK.md): «No crear `.env` ni mostrar secretos, cookies o cadenas de conexión» y autorización específica del ambiente/cuentas/recursos antes de E2E.

## Alcance que debe aprobarse

| Parámetro | Propuesta concreta |
| --- | --- |
| Ambiente | `https://localhost/GestionDocumental-Docuarchi.net/`, solo pruebas; verificar DLL y JS servidos contra la entrega antes de crear datos. Activación/despliegue requiere autorización propia si fuera necesario. |
| Cuentas | Una cuenta propietaria autorizada para importar y una distinta para privacidad; identidades por confirmar. No crear, activar ni cambiar permisos de cuentas. Una cuenta MySQL exclusivamente SELECT para controles RA/DA, entregada de forma efímera. |
| Destino/perfil | Nivel de pruebas PRUEBA-IMPORT y perfil publicado Egresos; nivel, perfil, versión, gabinete, empresa y área se fijan mediante lectura autorizada antes de confirmar. Si no corresponden, STOPPED. No modificar/publicar perfiles. |
| Corpus | Dos raíces físicas homónimas seleccionadas por separado,27 PDF sintéticos de677 bytes:14 en una raíz y13 en la otra;18279 bytes totales. Código resuelto igual entre raíces y presente previamente, comprobado con SELECT dentro del ámbito autorizado. Si la versión no permite resolverlo, detener y redefinir la ventana, sin editar reglas. |
| Efectos máximos | Una preparación, una confirmación, un lote;27 recepciones,2 expedientes/relaciones nuevas,27 materializaciones y27 documentos almacenados en su gabinete. No reutilizar expedientes anteriores. Sin cambios en esos expedientes. |
| Topes globales |1200 solicitudes HTTP intentadas,80 SELECT de control intentados y2 sesiones de aplicación. Recargar conserva la sesión propietaria; expirar/requerir otra autenticación detiene la ventana. Se cuentan errores y no se reinician presupuestos. |
| Disco/bytes |18279 bytes de PDF recibidos;512 MiB libres como mínimo antes de empezar y20 MiB de crecimiento observado máximo en los destinos de prueba acordados. Registrar por separado tamaño de PDF, transporte y derivados; no prometer que sean iguales. Si no se puede medir, no iniciar almacenamiento. |
| Fallos | Pausa/cierre/reapertura por interfaz. Sin inyección de pérdida de red, DDL, errores de disco, revocaciones, procesos detenidos ni concurrencia de escritores reales. |
| Retención | Conservar datos/evidencias en ubicaciones locales excluidas. Cero borrado, limpieza, SQL de escritura o jobs; cualquier retiro posterior requiere otro alcance. |

## Secuencia controlada

1. Crear runId y comprobar ambiente, fuentes servidas, recursos, clasificación/tipología, recepción, almacenamiento y espacio. Solo SELECT acotados para observar índices de expediente_archivo y datos autorizados; detectar cualquier UNIQUE independiente que impida repetir código. No eliminar índices ni alterar tablas.
2. Con la propietaria, seleccionar perfil y ambas raíces, preparar/revisar código y nombre, y pulsar **Confirmar e importar** una sola vez. Observar llamadas secuenciales y barreras del servidor. No usar el antiguo ejecutor DOC119 de preparación: su permiso excluía almacenamiento.
3. Pausar después de una recepción confirmada, dejar concluir la solicitud iniciada y comprobar que no aparece el siguiente efecto. Cerrar/reabrir el mismo lote; la consulta no ejecuta operaciones. Vincular los originales pendientes de ese lote y continuar explícitamente. Ningún recibo/expediente/documento confirmado puede repetirse.
4. Completar el mismo lote; comprobar dos IDs distintos con igual código, una relación por raíz y27 documentos vinculados al padre correspondiente. Consultar filtros, segunda página, vacío y CSV sin nuevos efectos. **Ver expedientes creados** debe usar IDs; comprobar etiqueta por código y navegación nativa, sin sustituir destinos ocultos.
5. Con la segunda cuenta, comprobar denegación de consulta y CSV del mismo lote. Terminar con SELECT de control y comparación de identidades, estados e inventario; no usar igualdad de huellas antes/después como criterio de ausencia de mutación, porque esta ventana sí autorizaría efectos concretos.

## Parada y recuperación

Ante acuse perdido, revisión incoherente, falta de autorización/catálogo/original, incremento de efectos, límite o error: **STOPPED**, conservar referencias y consumo, eliminar secretos al cerrar; cero repetición automática. Una lectura del resultado incierto consume el mismo presupuesto. No ofrecer reenviar un efecto no confirmado. La continuación de una ventana detenida requiere instrucción expresa y conserva consumos previos.

Registrar SHA/fuentes servidas, runId, conteos/IDs autorizados saneados, resultados por caso y límites; nunca cuerpos privados, credenciales, cookies o conexiones. Esta ventana sería aceptación funcional acotada; no INT-03 de capacidad, SII real, limpieza ni aceptación integral de DOC-120.
