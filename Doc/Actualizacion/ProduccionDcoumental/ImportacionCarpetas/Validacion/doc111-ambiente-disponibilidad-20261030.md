# DOC-111: disponibilidad del ambiente histórico E2E

Fecha: 2026-10-30. Comprobación solicitada por el usuario, sin autenticación ni cargas.

Ambiente: `https://localhost/GestionDocumental-Docuarchi.net/`.

| Comprobación | Resultado |
| --- | --- |
| IIS, aplicación `/GestionDocumental-Docuarchi.net` | Configurada en `Default Web Site`, pool `DefaultAppPool` |
| Directorio físico | `D:\imagenesda\DocuachiNet\DocuArchiNet`, coincide con este workspace |
| Servicios W3SVC y WAS | En ejecución |
| GET raíz HTTPS | HTTP 200, HTML |
| GET `gestor.aspx` | HTTP 200; formulario, campo password y ViewState presentes |
| GET `Gestion/WebFormProducionDocumental.aspx` | HTTP 200, HTML; no acredita autoridad ni funcionalidades de una sesión autenticada |
| TLS | TLS 1.3; certificado autofirmado rechazado por validación estricta de Node (`DEPTH_ZERO_SELF_SIGNED_CERT`) |
| Perfiles históricos de visor y DOC-103 | `ignoreHttpsErrors: true`, consistente con el certificado observado |

Las solicitudes no siguieron redirecciones ni enviaron credenciales. Se descartaron cuerpos HTML y cabeceras de sesión; solo se conservaron estados y banderas. No se ejecutó JavaScript del navegador, POST, SQL, prueba autenticada, almacenamiento o despliegue. Los GET pueden producir registros normales del servidor.

Conclusión: sitio y acceso disponibles en la URL histórica. Esto no verifica conexión de negocio, permisos de cuenta, esquema DOC-111, equivalencia del binario desplegado ni aceptación REC. La comprobación de disponibilidad no sustituye esas verificaciones.
