# DOC-120 — ventana propuesta de prerrequisitos del lote39

Estado: PREPARADA_LOCALMENTE; autorización solicitada, sin respuesta ni ejecución real.

Objeto: lote39, carpeta60, intento96 actualmente RESERVADO, en los DSN de pruebas RA/DA MembershipUsers elegidos expresamente por el operador. Hasta30 SELECT en total, incluyendo una lectura de metadatos unidos por lote/raíz/intento y las consultas de los catálogos existentes. Cero HTTP/sesiones de aplicación/recepciones/creaciones/almacenamiento/reconciliación/permisos/DDL/borrado. Conserva los6 intentos anteriores: máximo36 acumulado. Una parada cierra sin retry automático.

El selector compila un auxiliar C# excluido de Git contra la DLL local; registra huellas antes/después. La primera consulta exige las referencias exactas y la reserva sin evidencia/identidad; un cambio detiene. Se leen snapshot, valores y reclamación únicamente en memoria para construir objetos existentes. Ningún secreto de configuración, cookie o conexión del sitio se recupera. El delegado de catálogo solo comunica SELECT con parámetros, origen RA/DA y tablas permitidas al puente queryRows; el proceso C# no compone escritor/repositorio mutante.

Se elimina exclusivamente en la copia local la señal posterior de detención y se proyecta EsNuevaReserva para validar el comando previo al inicio. Es un contrafactual explícito: no demuestra autoridad HTTP, compatibilidad de la conexión legacy de aquella sesión, transacción de inicio ni que el efecto documental no exista. Se contrastan los catálogos actuales, no se reconstruye su estado histórico. Un PASS local o del catálogo no permite liberar ni repetir96.

Salida: etapa/código de lista cerrada, origen/tablas/conteos de lecturas, huellas, presupuesto y cierre. No filas, nombres, valores, rutas originales, tokens, SQL privado o credenciales. Runtime saneado en `.tmp/doc120-prerequisites/<runId>/state.json`, excluido. Se solicitan DSN y usuario/clave MySQL SELECT por consola enmascarada; credenciales eliminadas por lectura y al cerrar.

Comando, exclusivamente tras la autorización de esta ventana:

```powershell
node tools/e2e/scripts/inspect-doc120-creation-prerequisites.cjs --lot 39 --root 60 --attempt 96 --authorized
```

Regresión local23/23, incluida ejecución de las clases reales con registros sintéticos; evidencia `doc120-prerrequisitos-preparacion-20261114.json`. La corrección de causa3.31 necesita un fallo concreto reproducido; no se cambia producto por una hipótesis.
