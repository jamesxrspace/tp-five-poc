mkdir .\scripts\doc\documents
xcopy /y /s ..\..\one-schema\openapi-doc\game-server\documents .\scripts\doc\documents
rmdir /s /q .\packages\tpfive_game_server_api_client\lib
cd scripts && npx openapi-merge-cli && cd .. && flutter clean && flutter pub get && dart run build_runner build --delete-conflicting-outputs --build-filter="lib/generated/openapi/*.dart" && rmdir /s /q .\scripts\doc