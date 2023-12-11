import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class RecordButton extends ConsumerStatefulWidget {
  final VoidCallback onTap;
  const RecordButton({super.key, required this.onTap});

  @override
  RecordButtonState createState() => RecordButtonState();
}

class RecordButtonState extends ConsumerState<RecordButton> {
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
    return Center(
      child: GestureDetector(
        onTap: widget.onTap,
        child: Container(
          decoration: BoxDecoration(
            color: Colors.transparent,
            shape: BoxShape.circle,
            border: Border.all(
              color: Colors.white,
              width: 3.0,
            ),
          ),
          child: Container(
            decoration: BoxDecoration(
              color: Colors.transparent,
              shape: BoxShape.circle,
              border: Border.all(
                color: Colors.transparent,
                width: 2.0,
              ),
            ),
            child: Container(
              width: 60.0,
              height: 60.0,
              decoration: const BoxDecoration(
                color: Colors.red,
                shape: BoxShape.circle,
              ),
            ),
          ),
        ),
      ),
    );
  }
}
