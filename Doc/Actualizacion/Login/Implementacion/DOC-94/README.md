# Implementación DOC-94

DOC-94 separa la validación de credenciales de la finalización del login privado e incorpora la decisión de preautenticación para DocuArchi, Gestor, Radicación y Workflow.

La implementación solo lee estructuras existentes: catálogo central `empresa_gestion_documental`/`gestor_modulos` y las tablas `usuarios_da`, `remit_dest_interno`, `usuario_radicador` y `usuario_workflow`. No agrega tablas, columnas ni migraciones.

## Resultado funcional

- `RequiereSegundoFactor = 0` o `NULL`: resuelve ID/login sin exigir correo, ejecuta una vez el finalizador legacy y conserva la redirección actual.
- `RequiereSegundoFactor = 1`: exige correo válido, devuelve internamente `SECOND_FACTOR_REQUIRED` y se detiene antes de sesiones autenticadas, permisos, relaciones, auditoría y cookie.
- Configuración, módulo o identidad inválidos: falla cerrado con código sanitizado.

DOC-94 no crea challenge, no envía correo, no expone ASMX y no modifica markup o JavaScript OTP. Por ello no debe desplegarse con módulos activos en `1` hasta completar las Jiras posteriores.

## Documentos

- [Arquitectura y diagramas](01-ARQUITECTURA-Y-DIAGRAMAS.md)
- [Casos de uso](02-CASOS-DE-USO.md)
- [Inventario técnico](03-INVENTARIO-TECNICO.md)
- [Validación y pendientes](04-VALIDACION-Y-PENDIENTES.md)
- Contrato estructural: `diagram-contract.json`
- Diagramas obligatorios: `Diagramas/*.mmd`
