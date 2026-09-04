DO $bootstrap$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
        CREATE ROLE murimios_app
            LOGIN
            PASSWORD 'murimios-app-dev'
            NOSUPERUSER
            NOCREATEDB
            NOCREATEROLE
            NOINHERIT;
    END IF;
END
$bootstrap$;

GRANT CONNECT ON DATABASE murimios TO murimios_app;

CREATE SCHEMA IF NOT EXISTS platform;
CREATE SCHEMA IF NOT EXISTS access;
CREATE SCHEMA IF NOT EXISTS farm;
CREATE SCHEMA IF NOT EXISTS production;
CREATE SCHEMA IF NOT EXISTS operations;
CREATE SCHEMA IF NOT EXISTS inventory;
CREATE SCHEMA IF NOT EXISTS evidence;
CREATE SCHEMA IF NOT EXISTS reporting;
CREATE SCHEMA IF NOT EXISTS livestock;
CREATE SCHEMA IF NOT EXISTS sync;
CREATE SCHEMA IF NOT EXISTS messaging;
CREATE SCHEMA IF NOT EXISTS notifications;
CREATE SCHEMA IF NOT EXISTS configuration;
CREATE SCHEMA IF NOT EXISTS traceability;
CREATE SCHEMA IF NOT EXISTS supply;
CREATE SCHEMA IF NOT EXISTS planning;
CREATE SCHEMA IF NOT EXISTS finance;
CREATE SCHEMA IF NOT EXISTS integrations;
CREATE SCHEMA IF NOT EXISTS insights;
CREATE SCHEMA IF NOT EXISTS imports;
CREATE SCHEMA IF NOT EXISTS compliance;
CREATE SCHEMA IF NOT EXISTS connected;
CREATE SCHEMA IF NOT EXISTS analytics;
CREATE SCHEMA IF NOT EXISTS external_api;
CREATE SCHEMA IF NOT EXISTS retail;
CREATE SCHEMA IF NOT EXISTS enterprise;

GRANT USAGE ON SCHEMA access, platform, farm, production, operations, inventory, evidence, reporting, livestock, sync, messaging, notifications, configuration, traceability, supply, planning, finance, integrations, insights, imports, compliance, connected, analytics, external_api, retail, enterprise TO murimios_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA access, platform, farm, production, operations, inventory, evidence, reporting, livestock, sync, messaging, notifications, configuration, traceability, supply, planning, finance, integrations, insights, imports, compliance, connected, analytics, external_api, retail, enterprise TO murimios_app;
GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA access, platform, farm, production, operations, inventory, evidence, reporting, livestock, sync, messaging, notifications, configuration, traceability, supply, planning, finance, integrations, insights, imports, compliance, connected, analytics, external_api, retail, enterprise TO murimios_app;

ALTER DEFAULT PRIVILEGES IN SCHEMA access, platform, farm, production, operations, inventory, evidence, reporting, livestock, sync, messaging, notifications, configuration, traceability, supply, planning, finance, integrations, insights, imports, compliance, connected, analytics, external_api, retail, enterprise
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO murimios_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA access, platform, farm, production, operations, inventory, evidence, reporting, livestock, sync, messaging, notifications, configuration, traceability, supply, planning, finance, integrations, insights, imports, compliance, connected, analytics, external_api, retail, enterprise
    GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO murimios_app;
