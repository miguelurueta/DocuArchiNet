# 03 — Persistencia central, migración y concurrencia

## ROL ESPERADO

Actúa como arquitecto de datos MySQL y desarrollador VB.NET orientado a concurrencia segura.

## OBJETIVO

Versionar el endurecimiento de `docuarchi.ra_auth_second_factor_challenge` e implementar un repositorio central con operaciones atómicas.

## RESTRICCIONES CRITICAS

- Leer y aplicar `00-contexto-obligatorio.md` y los contratos de la etapa 02.
- Preparar una migración versionada, reversible y compatible; no ejecutarla en ningún ambiente sin autorización.
- No modificar copias de `gestor_modulos` en esquemas satélite.
- No guardar OTP, secreto HMAC ni contexto completo.
- No implementar lectura-seguida-de-escritura vulnerable para intentos o consumo.

## REQUISITOS POSITIVOS

1. Comparar el DDL propuesto con el esquema físico verificado y producir migración idempotente o con precondiciones explícitas.
2. Añadir solo los campos/índices aprobados para propósito, vínculo de sesión, revocación/consumo, concurrencia y limpieza.
3. Conservar compatibilidad con filas anteriores; definir su tratamiento fail-closed.
4. Implementar creación con invalidación controlada de challenges activos equivalentes.
5. Implementar verificación de estado, expiración e intentos y consumo mediante transacción/bloqueo o `UPDATE` condicional con comprobación exacta de filas afectadas.
6. Implementar reenvío con límites aprobados e invalidación del código anterior.
7. Definir consulta/lote de limpieza de expirados sin integrarlo todavía a un scheduler no existente.
8. Usar siempre la conexión central explícita, nunca una conexión de módulo retenida accidentalmente en Session.

## CRITERIOS DE ACEPTACION

- Dos verificaciones concurrentes no consumen el mismo challenge dos veces.
- No se pierden incrementos de intentos.
- Un challenge de otra sesión, propósito, empresa, módulo o usuario no es válido.
- El repositorio no devuelve SQL, secretos ni excepciones internas.
- Rollback del DDL y compatibilidad quedan documentados.

## PRUEBAS OBLIGATORIAS

Agregar pruebas de repositorio e integración para creación, unicidad, invalidación, expiración, intentos, consumo concurrente y datos legacy. Las pruebas de escritura deben usar una base descartable autorizada; si no existe, validar scripts estáticamente y documentar el bloqueo sin afirmar ejecución.

## ENTREGABLE FINAL

Entregar migración/rollback, repositorio, pruebas, evidencia de atomicidad y riesgos operativos. No aplicar el DDL ni avanzar si el diseño permite doble consumo.

## Correcciones opsxj:prompt-review

Estas reglas proceden de `opsxj:prompt-review` y fueron ajustadas a ASP.NET Web Forms, VB.NET, MySQL y la infraestructura real de este repositorio.

## Contexto obligatorio
Leer `00-contexto-obligatorio.md`, la exploración, los entregables de etapas previas y el código vigente de `gestor.aspx`, `Defaul`, `Modelo`, `DTOs`, `Services`, `Infrastructure`, `webservice`, proyecto y pruebas que resulten afectados.

## Pruebas obligatorias
Ejecutar pruebas focales con la infraestructura existente, `msbuild .\\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug` cuando esté disponible y Playwright solo en las etapas y ambientes expresamente autorizados; registrar comandos, códigos de salida y resultados reales.

## Documentacion tecnica
Actualizar únicamente el paquete canónico `Doc/Actualizacion/Login/Implementacion/` y la exploración cuando cambie una decisión comprobada.

## Entregable final
Entregar archivos modificados, pruebas y compilación ejecutadas, documentación, evidencia sanitizada, limitaciones y riesgos coherentes con lo realmente implementado en la etapa.

Preservar firmas y recorridos actuales no relacionados; prohibir endpoints alternos de credenciales, bypass del OTP, duplicación SMTP, SQL dentro del ASMX y cambios silenciosos de configuración para hacer pasar pruebas.

Exigir MSBuild del proyecto WebForms afectado y registrar comando, código de salida y errores; si MSBuild no está disponible, documentar el bloqueo y una verificación reproducible sin afirmar éxito.

Usar las pruebas existentes del repositorio y `node --test` para contratos CJS cuando corresponda; no introducir Vitest, Testing Library ni otro runner sin necesidad técnica aprobada.

Esta etapa no cierra por sí sola el cambio: el E2E real forma parte integral del cierre en la etapa 09. Antes de esa autorización, ejecutar pruebas focales y documentar formalmente por qué no se realizó todavía una corrida autenticada.
