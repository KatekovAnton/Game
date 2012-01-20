import clr
import sys
clr.AddReference("PhysX_test2")
from PhysX_test2 import *
from System import *
from System.Math import *

# импортирование dll-ок
# sys.path.Add(GameConfiguration.AppPath)
# clr.AddReferenceToFile("some.dll")


# создание необходимых функций
# это похоже на враппер
# желательно использовать функции, объ€вленные здесь, так как возможно их прототипы будут изменены
def Ex(obj) : SE.Instance.Execute(obj)
def ExScript(obj) : SE.Instance.ExScript(obj)