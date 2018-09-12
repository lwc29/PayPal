@echo on   
color 2f   
mode con: cols=80 lines=25   
@REM   
@echo 正在清理SVN文件，请稍候......   

  
for /r . %%a in (.) do @if exist "%%a\*.pdb" del /s /f "%%a\*.pdb"  
@echo 清理完毕！！！   
@pause  