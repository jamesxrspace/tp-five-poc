import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tpfive/providers/loading_config_provider.dart';

class FullScreenLoading extends ConsumerStatefulWidget {
  const FullScreenLoading({super.key});

  @override
  FullScreenLoadingState createState() => FullScreenLoadingState();
}

class FullScreenLoadingState extends ConsumerState<FullScreenLoading> {
  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final loadingState = ref.watch(loadingStateProvider);
    final unityGeneralMessage = ref.watch(unityGeneralMessageProvider);
    return Container(
      color: Colors.black.withOpacity(loadingState.config.alpha),
      child: Stack(alignment: loadingState.config.position, children: [
        SizedBox(
          height: 80,
          child: Padding(
            padding: const EdgeInsets.all(8.0),
            child: Column(
              children: [
                SizedBox(
                  width: 40,
                  height: 40,
                  child: CircularProgressIndicator(
                    backgroundColor: Colors.white.withOpacity(0.2),
                    color: Theme.of(context).primaryColor,
                    strokeWidth: 3,
                  ),
                ),
                const SizedBox(height: 5),
                if (unityGeneralMessage.isNotEmpty)
                  Text(unityGeneralMessage,
                      style: TextStyle(color: Theme.of(context).primaryColor)),
              ],
            ),
          ),
        ),
      ]),
    );
  }
}
