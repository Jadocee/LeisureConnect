#!/bin/bash
#
# SQL Server Initialization Script
# This script waits for SQL Server to be ready and applies initialization scripts
# if they haven't been applied before.

set -e  # Exit immediately if a command exits with a non-zero status

# Configuration
FLAG_FILE="/var/opt/mssql/.init_done"
MAX_RETRIES=60
RETRY_INTERVAL=5
SQL_SERVER="leisureconnect-db"
SCRIPTS_DIR="/init/scripts"

# Display timestamp in logs
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1"
}

# Check if required environment variables are set
check_requirements() {
    if [[ -z "$SA_PASSWORD" ]]; then
        log "ERROR: SA_PASSWORD environment variable must be set"
        exit 1
    fi
}

# Wait for SQL Server to be ready
wait_for_sqlserver() {
    log "Waiting for SQL Server to be ready..."
    
    for ((i=1; i<=MAX_RETRIES; i++)); do
        if /opt/mssql-tools/bin/sqlcmd -h -1 -t 1 -S "$SQL_SERVER" -U "SA" -P "$SA_PASSWORD" \
           -Q "SET NOCOUNT ON; SELECT 1" -C &>/dev/null; then
            log "SQL Server is up and running"
            return 0
        fi
        
        log "Attempt $i/$MAX_RETRIES: SQL Server is not ready yet. Retrying in $RETRY_INTERVAL seconds..."
        sleep $RETRY_INTERVAL
    done
    
    log "ERROR: Could not connect to SQL Server after $MAX_RETRIES attempts"
    return 1
}

# Execute SQL scripts
execute_sql_scripts() {
    local script_count=0
    local success_count=0
    
    for file in "$SCRIPTS_DIR"/*.sql; do
        if [[ -f "$file" ]]; then
            ((script_count++))
            log "Executing script: $(basename "$file")"
            
            if /opt/mssql-tools/bin/sqlcmd -S "$SQL_SERVER" -U "SA" -P "$SA_PASSWORD" -i "$file" -C; then
                log "Script $(basename "$file") executed successfully"
                ((success_count++))
            else
                log "ERROR: Failed to execute script $(basename "$file")"
                return 1
            fi
        fi
    done
    
    if [[ $script_count -eq 0 ]]; then
        log "No SQL scripts found in $SCRIPTS_DIR"
    else
        log "Executed $success_count out of $script_count scripts successfully"
    fi
    
    return 0
}

# Create flag file to indicate initialization is complete
mark_as_initialized() {
    touch "$FLAG_FILE"
    log "Initialization complete. Flag file created at $FLAG_FILE"
}

# Main function
main() {
    log "Starting SQL Server initialization process"
    check_requirements
    
    if wait_for_sqlserver; then
        if [[ ! -f "$FLAG_FILE" ]]; then
            log "First-time initialization detected. Running setup scripts..."
            if execute_sql_scripts; then
                mark_as_initialized
                log "Initialization completed successfully"
            else
                log "ERROR: Initialization failed"
                exit 1
            fi
        else
            log "Initialization already completed. Skipping setup scripts."
        fi
    else
        log "ERROR: SQL Server failed to start properly"
        exit 1
    fi
}

# Execute main function
main