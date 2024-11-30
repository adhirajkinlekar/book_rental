dotnet ef dbcontext scaffold "Name=ConnectionStrings:BRDBConnection" Microsoft.EntityFrameworkCore.SqlServer --project=book_rental_api  --output-dir ./Data/book_rental_db

If you're okay with overwriting the files (e.g., to update them), use the --force flag.  
