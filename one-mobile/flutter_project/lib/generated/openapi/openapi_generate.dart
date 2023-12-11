// Openapi Generator last run: : 2023-09-11T14:51:55.979656
import 'package:openapi_generator_annotations/openapi_generator_annotations.dart';

@Openapi(
    additionalProperties: AdditionalProperties(
        pubName: 'tpfive_game_server_api_client',
        pubAuthor: 'TPFive',
        pubDescription: 'TPFive Game Server Api Client',
        pubVersion: '1.0.0'),
    debugLogging: true,
    inputSpecFile: './scripts/doc/documents/app/_merge_api.yaml',
    templateDirectory: './scripts/openapi_template',
    generatorName: Generator.dart,
    overwriteExistingFiles: true,
    alwaysRun: true,
    outputDirectory: 'packages/tpfive_game_server_api_client')
class TPFiveApiConfig extends OpenapiGeneratorConfig {}
