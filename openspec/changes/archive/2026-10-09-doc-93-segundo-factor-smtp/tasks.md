<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
# Tareas atómicas — DOC-93

## 1. Contratos y modelos

- [x] 1.1 [M] Crear modelos inmutables de configuración, resolución y entrega SMTP con los cinco estados aprobados. Área/archivos: `Modelo/Login/SegundoFactor/SegundoFactorSmtpModels.vb`. Origen: D-03, RQ-03. Verificación: prueba de constructores, inmutabilidad y estados.
- [x] 1.2 [S] Extender puertos SMTP sin retirar ni cambiar `ISecondFactorEmailSender.Send`. Área/archivos: `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb`. Origen: D-02, RQ-02. Verificación: prueba estructural de firmas DOC-91/DOC-93.

## 2. Repositorio de configuración

- [x] 2.1 [M] Implementar lectura con fábrica/ejecutor compartidos, contexto defensivo y columnas explícitas parametrizadas. Área/archivos: `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorSmtpConfigurationRepository.vb`. Origen: D-04, RQ-04. Verificación: doble ADO.NET confirma SQL, parámetro y disposición de conexión.
- [x] 2.2 [S] Resolver cero, una y múltiples filas sin `LIMIT 1` ni selección implícita. Área/archivos: repositorio SMTP. Origen: D-05, RQ-05. Verificación: casos `Disabled`, configuración única y `AmbiguousConfiguration`.
- [x] 2.3 [M] Validar nulos, formato, rangos, banderas y credenciales antes de construir configuración utilizable. Área/archivos: repositorio y modelos SMTP. Origen: D-06, RQ-06. Verificación: matriz inválida retorna `InvalidConfiguration` sin crear cliente.
- [x] 2.4 [S] Implementar conversión comprobada y cota del timeout preservando dominio sin usarlo en credenciales. Área/archivos: repositorio/modelos SMTP. Origen: D-07, RQ-07. Verificación: valores normal, acotado, no positivo y overflow.

## 3. Transporte y fachada

- [x] 3.1 [M] Implementar fábrica/adaptador descartable de `SmtpClient` que aplique host, puerto, timeout, SSL y credenciales. Área/archivos: `Infrastructure/Login/SegundoFactor/Smtp/FrameworkSmtpClientAdapter.vb`. Origen: D-08, RQ-08. Verificación: doble captura opciones y confirma `Dispose` en éxito/error.
- [x] 3.2 [M] Construir `MailMessage` OTP mínimo con asunto fijo, remitente y destinatario validados. Área/archivos: `Infrastructure/Login/SegundoFactor/Smtp/SecondFactorSmtpTransport.vb`. Origen: D-08, RQ-08. Verificación: cuerpo contiene solo código, expiración y advertencias; recursos liberados.
- [x] 3.3 [M] Implementar `SecondFactorSmtpEmailSender` sobre repositorio/transporte y mapear estados al contrato DOC-91. Área/archivos: `Infrastructure/Login/SegundoFactor/Smtp/SecondFactorSmtpEmailSender.vb`. Origen: D-02, RQ-02. Cobertura adicional: D-09, RQ-09. Verificación: cada estado produce `Success/PublicCode` aprobado.
- [x] 3.4 [S] Sanitizar todas las excepciones y evitar registros o retornos con secretos. Área/archivos: fachada, repositorio y transporte SMTP. Origen: D-09, RQ-09. Verificación: excepciones sintéticas no filtran marcadores secretos.

## 4. Integración de proyecto y antirregresión

- [x] 4.1 [S] Registrar una sola vez las nuevas fuentes VB.NET sin agregar paquetes. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-01, RQ-01. Verificación: MSBuild resuelve todas las fuentes y no cambia `packages.config`.
- [x] 4.2 [M] Crear prueba estructural de rutas, SQL prohibido, ausencia de acoplamiento HTTP y correo legacy intacto. Área/archivos: `tests/login-second-factor-smtp.test.cjs`. Origen: D-01, RQ-01. Cobertura adicional: D-04, D-10, RQ-04, RQ-10. Verificación: Node test falla ante cambios en `ClassCorreo` o invocaciones legacy.
- [x] 4.3 [L] Crear runner conductual VB.NET con dobles sin red para repositorio, transporte, fachada y disposición. Área/archivos: `tests/LoginSecondFactorSmtpBehaviorTests.cs` y runner temporal. Origen: D-05, RQ-05. Cobertura adicional: D-06, D-07, D-08, D-09, RQ-06, RQ-07, RQ-08, RQ-09. Verificación: matriz completa en código 0 sin sockets.
- [x] 4.4 [M] Ejecutar regresiones DOC-91/DOC-92, pruebas DOC-93 y MSBuild. Área/archivos: suites existentes y proyecto. Origen: D-10, RQ-10. Verificación: comandos y códigos registrados; ningún envío real.

## 5. Documentación y cierre técnico

- [x] 5.1 [M] Documentar arquitectura, flujo, inventario de clases/firmas/columnas, errores y límites de autorización. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-93/` y `Doc/Tecnica/Opsxj/doc-93-segundo-factor-smtp/`. Origen: D-03, RQ-03. Cobertura adicional: D-04, D-08, D-10, RQ-04, RQ-08, RQ-10. Verificación: documentación coincide con firmas implementadas y declara ausencia de SMTP real.
- [x] 5.2 [S] Validar trazabilidad D/RQ, OpenSpec estricto y diff de correo legacy. Área/archivos: cambio OpenSpec DOC-93 y Git. Origen: D-10, RQ-10. Verificación: `opsxj:refine`, OpenSpec y revisión de diff terminan en código 0.
