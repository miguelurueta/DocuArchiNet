# Prompt profesional — OpenSpec + Validación Automática (RecoveryPassword)

> Copia textual de requerimientos funcionales/técnicos solicitados para implementar validación automática entre OpenSpec y tests en RecoveryPassword.

Repositorio: DocuArchiCore.react  
Rol: Arquitecto de Software y Desarrollador Frontend Senior (React + TypeScript + SPA)

## Objetivo
Implementar un sistema de validación automática entre OpenSpec y pruebas del módulo RecoveryPassword.

## Reglas clave
- Cada escenario OpenSpec debe tener al menos un test.
- Ningún test puede usar un SPEC ID inexistente.
- Reporte legible con faltantes/errores.
- Exit code 1 cuando exista desalineación.
- Integrable en CI y scripts npm.

## Alcance de escenarios críticos
- Solicitud de recuperación SUCCESS.
- Email inválido/usuario inexistente (funcional).
- Error técnico API.
- Reset SUCCESS con token válido.
- Token inválido/expirado/reutilizado.
- Validaciones UI de password.
- Navegación SPA sin recarga.
