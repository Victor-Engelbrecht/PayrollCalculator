#!/bin/bash
set -e

SQLCMD=/opt/mssql-tools18/bin/sqlcmd
CONN="-S ${DB_SERVER:-sqlserver,1433} -U sa -P $SA_PASSWORD -No -b"

echo "Creating database..."
$SQLCMD $CONN -Q "IF DB_ID('PayrollCalculator') IS NULL CREATE DATABASE PayrollCalculator;"

echo "Running migrations..."
for f in $(ls /scripts/*.sql | sort); do
    echo "  $(basename $f)..."
    $SQLCMD $CONN -d PayrollCalculator -i "$f"
done

echo "Migrations complete."
