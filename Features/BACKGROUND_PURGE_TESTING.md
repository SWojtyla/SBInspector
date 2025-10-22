# Testing Guide for Background Purge Operation

## Test Scenarios

### Scenario 1: Basic Background Purge
**Objective**: Verify that purge operation runs in background without blocking UI

**Steps**:
1. Connect to a Service Bus instance
2. Navigate to a queue or subscription with messages
3. Click "View Messages" for Active, Scheduled, or Dead-Letter messages
4. Click the "Purge All" button
5. Type "PURGE" in the confirmation dialog and click "Purge All"

**Expected Results**:
- ✅ Confirmation dialog closes immediately
- ✅ Background operation panel appears in bottom-right corner
- ✅ Panel shows "Purging Messages" with entity name
- ✅ Progress counter updates in real-time showing deleted message count
- ✅ UI remains responsive - you can click on other entities
- ✅ Upon completion, success message appears
- ✅ Message list is cleared
- ✅ Entity counts in tree view are refreshed
- ✅ Background panel disappears

### Scenario 2: Cancel During Purge
**Objective**: Verify cancellation functionality works correctly

**Steps**:
1. Start a purge operation on a queue/subscription with many messages (100+)
2. While the operation is running, click the "Cancel" button in the background panel

**Expected Results**:
- ✅ Operation stops immediately
- ✅ Background panel disappears
- ✅ Success message shows: "Purge cancelled. Deleted X message(s) before cancellation."
- ✅ Messages that were deleted remain deleted
- ✅ Remaining messages are still visible in the queue/subscription

### Scenario 3: UI Navigation During Purge
**Objective**: Verify UI remains functional during background operation

**Steps**:
1. Start a purge operation on a queue
2. While operation is running:
   - Navigate to a different queue
   - View messages in the other queue
   - Expand topics in the tree view
   - Click on subscriptions

**Expected Results**:
- ✅ All navigation works normally
- ✅ Background panel remains visible showing purge progress
- ✅ You can view and interact with other entities
- ✅ Original purge operation continues in background

### Scenario 4: Multiple Message Types
**Objective**: Verify purge works for all message types

**Steps**:
1. Test purge on Active messages
2. Test purge on Scheduled messages
3. Test purge on Dead-Letter messages
4. Test purge on Subscription messages (all types)

**Expected Results**:
- ✅ Each message type can be purged successfully
- ✅ Progress updates correctly for each type
- ✅ Correct message type is shown in progress panel
- ✅ Entity counts update appropriately after purge

### Scenario 5: Empty Queue/Subscription
**Objective**: Verify behavior when no messages to purge

**Steps**:
1. Navigate to an empty queue or create one
2. View messages (will show empty)
3. Click "Purge All" and confirm

**Expected Results**:
- ✅ Operation completes quickly
- ✅ Error message: "No messages were found to purge."
- ✅ No errors or exceptions occur

### Scenario 6: Error Handling
**Objective**: Verify graceful error handling

**Steps**:
1. Start a purge operation
2. Disconnect network (if possible in test environment)
3. Or disconnect from Service Bus during purge

**Expected Results**:
- ✅ Error message is displayed
- ✅ Background panel is dismissed
- ✅ UI remains functional
- ✅ User can reconnect and try again

### Scenario 7: Cancel Confirmation Dialog
**Objective**: Verify canceling the confirmation dialog

**Steps**:
1. Click "Purge All" button
2. In the confirmation dialog, click "Cancel" or X

**Expected Results**:
- ✅ Dialog closes
- ✅ No purge operation starts
- ✅ Messages remain unchanged

## Visual Verification

### Background Operation Panel
- **Position**: Fixed at bottom-right corner (20px from edges)
- **Size**: Max width 400px, auto height
- **Animation**: Slides in from right with smooth transition
- **Content**:
  - Spinning arrow icon (continuously rotating)
  - Title: "Purging Messages"
  - Message: Shows entity name and message type
  - Progress bar: Animated striped bar showing count
  - Cancel button: Red outline, visible at all times

### Progress Updates
- Count should update smoothly (not jump by large amounts)
- Progress bar remains animated throughout operation
- Text shows exact count: "247 deleted"

## Performance Testing

### Large Message Count
Test with queues containing:
- 100 messages
- 1,000 messages
- 10,000+ messages (if available)

**Expected**:
- Progress updates regularly (approximately every 100 messages)
- UI remains responsive
- Memory usage is stable
- Operation can be cancelled at any point

## Browser Compatibility
Test in:
- Chrome/Edge
- Firefox
- Safari (if available)

All features should work consistently across browsers.

## Known Limitations

1. Only one purge operation can run at a time (by design)
2. Progress is reported in batches of ~100 messages
3. Cancelled operations may take a moment to fully stop
4. Network latency affects progress update frequency

## Success Criteria

The implementation is successful if:
- ✅ UI never blocks during purge operations
- ✅ Real-time progress is visible
- ✅ Cancel functionality works reliably
- ✅ All message types can be purged
- ✅ Error handling is graceful
- ✅ No console errors or exceptions
- ✅ Build succeeds without errors
- ✅ Code follows existing patterns and style
