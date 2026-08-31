# LECTURA-LISTADO-CONTADOR

- Ticket: DOC-41
- Cambio OpenSpec: doc-41-lectura-listado-contador
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- `unit`: 2026-08-31, la batería focal de contratos, sesión, recursos y
  plataforma E2E terminó con 40 pruebas aprobadas. Referencia: commit de
  entrega DOC-41 y salida local de `node --test` bajo `tools/e2e/tests`.
- `manual_qa`: 2026-08-31, recorrido autorizado de lectura de Notas Workflow
  validó acceso anónimo bloqueado y lectura autenticada sin cambios de estado
  ni auditoría. La evidencia conserva únicamente códigos, conteos, latencias y
  huellas saneadas.

## QA/E2E WebForms

La E2E automatizada de Notas se ejecuta solo con autorización explícita de
ambiente, cuenta y tarea. El arnés valida el certificado por defecto y permite
una excepción local efímera autorizada. No se almacenan credenciales, cookies,
respuestas HTTP ni contenido de Notas en los reportes.
