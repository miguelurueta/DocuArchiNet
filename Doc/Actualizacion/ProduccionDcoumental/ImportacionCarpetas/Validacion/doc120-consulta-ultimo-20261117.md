# DOC-120 — consulta del último lote,2026-11-17

Base5b0f4850. Solicitud: «pero ahora no me deja consultar el ultimo lote» y video `Grabación 2026-11-17 170531-bloqueo.mp4` de8,87s. Se observan el botón deshabilitado, ausencia de ID visible y selección posterior de Egresosv3 sin habilitarlo. Código: shortcutsRender deshabilitaba con `!model.ultimoLoteId` y el acceso de menú se ocultaba sin host.doc111UltimoLote. La referencia dependía de memoria/sessionStorage del contexto; no era una búsqueda de historial de la cuenta. No se certifica por qué desapareció la referencia ni se atribuye a un fallo HTTP observado.

[Contrato de la corrección](../Diseno/consulta-ultimo-lote-doc120.md).3.159–3.164 completas localmente;173/184 tareas y65 escenarios. Se mantienen las once abiertas anteriores.

## Implementación y verificación

- Acceso disponible sin perfil, nivel o ID guardado. Si falta ID, un clic envía ConsultarUltimoLote con solicitud vacía; si existe, conserva consulta de la referencia guardada. No se busca automáticamente al elegir perfil.
- SELECT parametrizado del solicitante/empresa actuales, orden por lote descendente y LIMIT1. Antes de exponer el ID, valida destino original, snapshot y recursos con la autoridad actual. No busca lotes de otra cuenta ni convierte denegación en ausencia.
- DTO/envelope, sesión/CSRF y composición de consulta existentes. Puerto aditivo conserva consumidores. No nuevas tablas/columnas, SQL correctivo, recepción, creación, almacenamiento o reanudación por consultar.
- Selección/perfil/plan conservados al consultar y volver. Menú sin referencia abre consulta; ausencia, fallo, servidor anterior y cierre conservan comportamiento seguro y alternativa manual. Cambiar contexto no hereda referencias anteriores.

**70/70 Node únicas**,68 cliente/Chromium y2 wrappers Framework/DDL. No se suman casos internos:

```text
node --test tests/doc120-consultar-ultimo.test.cjs tests/doc120-espera-reanudacion.test.cjs tests/doc120-parada-diagnostico.test.cjs tests/doc120-coordinacion-browser.test.cjs tests/doc111-importacion-carpetas-client.test.cjs
node --test tests/doc110-perfil-importacion-behavior.test.cjs
```

Siete pruebas nuevas Chromium cubren selección abierta, sin referencia/perfil/nivel, ausencia, servidor anterior, denegación y cierre. Framework cubre SELECT/parametrización, cuenta/empresa/permisos/destino, resultado vacío, IDs inválidos, campos inyectados, CSRF y envelope; dobles, sin SQL real. La regresión de cambio de contexto ahora comprueba acceso visible pero sin ID heredado, preservando aislamiento. Compilación completa targetCompile aislada en `.tmp/doc120-ultimo-build/`, cero errores y advertencias del proyecto. Fuentes/huellas/logs en [JSON compañero](doc120-consulta-ultimo-20261117.json).

## Estado operativo

Recursos cliente `doc120-ultimo-20261117-1`; la DLL compilada es `9a35cb3ed58cf60aa9e94db019b7820fa5e5123ad24f5eb02ee45768f5379a7d`. **Despliegue pendiente de autorización separada**: sustituir DLL/PDB solo en el sitio local, con respaldo/huellas y una comprobación HTTP anónima. Puede reiniciar la aplicación y su sesión; no reciclar el pool compartido. Se observó una DLL activa diferente del corte anterior, con fecha16:59:36 y hash `230391d329e6bab33b559dd04a4e7ad69fd1b403385a59e6805272d33443825f`; no se reemplaza ni se atribuye su origen, y deberá verificarse otra vez antes de instalar.

No se accedió a logs de IIS protegidos: la lectura fue denegada y la solicitud de ampliación fue interrumpida por el usuario antes de ejecutar. El video permite diagnosticar la condición de interfaz sin dichos logs. Esta corrección realiza0 HTTP al ambiente real,0 SELECT reales,0 sesiones autenticadas y0 efectos. No recupera el lote50 ni acredita la consulta de su cuenta en ejecución real. Para consultar mientras no se instale el método, usar Consultar importación y la referencia conocida; no crear otro lote para recuperar el anterior.
