# DOC-111: despliegue local autorizado

Autorización: el usuario respondió «autorizado» a reemplazar la DLL del sitio, con aviso de reinicio de la aplicación IIS. Alcance: solo el binario principal; no carga real.

Despliegue: 2026-10-30, 21:13:15 UTC. Origen: `.tmp/doc111-limpieza-build/bin/GestionDocumental-Docuarchi.net.dll`. Destino: `bin/GestionDocumental-Docuarchi.net.dll` del sitio IIS asociado a este workspace.

Antes del cambio se verificaron los 107 archivos del manifiesto de fuentes: ninguno cambió. El build aislado registrado finalizó con cero errores y 308 advertencias. Las referencias del binario que tienen una DLL presente en `bin` no mostraron diferencias de versión; este control no certifica por sí solo la resolución de todas las dependencias en ejecución.

El reemplazo se hizo mediante `File.Replace`, con copia anterior y comprobación de huellas:

| Archivo | SHA-256 |
| --- | --- |
| DLL activa desplegada | `2c1d704312fd8a225c49469a0ed1141589bdef479819830dff462450d3ced959` |
| Copia anterior: `.tmp/doc111-deployment-20261030-211315/previous.dll` | `25beb85ef62ed7c03a7b8739a90771def1e0a0d20dc356dc57ead76a86ca266d` |

La inspección de metadatos del destino confirma ahora la presencia de `PoliticaLimpiezaOriginalImportacion` y `ComposicionLimpiezaOriginalImportacion`, ausentes en la DLL anterior. No se modificaron configuración, permisos, perfiles ni dependencias durante este despliegue. El manifiesto SHA previo conserva las huellas binarias históricas; esta evidencia registra su sustitución autorizada.

## Arranque comprobado

A las 21:13:42 UTC, GET sin autenticar a `https://localhost/GestionDocumental-Docuarchi.net/`:

| Ruta | Resultado |
| --- | --- |
| Raíz | HTTP 200, formulario y campo de contraseña |
| `gestor.aspx` | HTTP 200, formulario y campo de contraseña |
| `Gestion/WebFormProducionDocumental.aspx` | HTTP 200, formulario |

Sin indicadores de página de error ASP.NET en las tres respuestas. Se usó la excepción al certificado autofirmado ya documentada; no se enviaron credenciales ni se guardaron cuerpos o cookies. Artefacto saneado: `tools/e2e/artifacts/doc111-deployment-result.json`.

Conclusión: reemplazo y arranque básico PASS. No acredita login, autorización HTTP, operaciones de importación ni REC. La carga real de los dos expedientes/documentos del escenario LOTE continúa pendiente de autorización y sesión interactiva. Progreso de aceptación: 119/130, sin cierre adicional.

Reversión, si se solicita o resulta necesaria por fallo atribuible al despliegue: restaurar exclusivamente la DLL anterior indicada, verificar su huella y repetir las comprobaciones de arranque. No se ejecutó reversión porque el arranque pasó.
